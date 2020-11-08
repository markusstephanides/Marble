using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Marble.Core.Declaration;
using Marble.Core.Messaging;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Models;
using Marble.Core.Transformers;
using Marble.Messaging.Rabbit.Models;
using Marble.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Marble.Messaging.Rabbit
{
    public class RabbitMessagingClient : IMessagingClient
    {
        private readonly RabbitClientConfiguration configuration;
        private readonly ILogger<RabbitMessagingClient> logger;
        private readonly IDictionary<string, ResponseAwaitation> responseAwaitations;

        private IModel channel;
        private IConnection connection;
        private MessagingFacade messagingFacade;

        public RabbitMessagingClient(IOptions<RabbitClientConfiguration> configurationOption,
            ILogger<RabbitMessagingClient> logger)
        {
            this.logger = logger;
            this.responseAwaitations = new Dictionary<string, ResponseAwaitation>();
            this.configuration = configurationOption.Value;
        }

        public async Task<TResult> SendAsync<TResult>(RequestMessage requestMessage)
        {
            return (TResult) await this.SendRoutedMessage(new RabbitRoutedMessage
            {
                Exchange = Utilities.AmqDirectExchange,
                MessageType = MessageType.RpcRequest,
                RoutingKey = ProcedurePath.FromRequestMessage(requestMessage),
                CorrelationId = Guid.NewGuid().ToString(),
                Payload = requestMessage.Arguments,
                Headers = new Dictionary<string, object>
                {
                    {"Controller", requestMessage.Controller},
                    {"Procedure", requestMessage.Procedure}
                }
            });
        }

        public Task SendAndForgetAsync(RequestMessage requestMessage)
        {
            return this.SendRoutedMessage(new RabbitRoutedMessage
            {
                Exchange = Utilities.AmqDirectExchange,
                RoutingKey = ProcedurePath.FromRequestMessage(requestMessage),
                Headers = new Dictionary<string, object>
                {
                    {"Controller", requestMessage.Controller},
                    {"Procedure", requestMessage.Procedure}
                },
                Payload = requestMessage.Arguments
            });
        }

        public void Connect(MessagingFacade messagingFacade, IEnumerable<ControllerDescriptor> controllerDescriptors)
        {
            this.messagingFacade = messagingFacade;
            this.logger.LogInformation($"Connecting to {this.configuration.ConnectionString}");

            if (this.connection != null)
            {
                throw new InvalidOperationException("Client is already connected!");
            }

            var factory = new ConnectionFactory
            {
                Uri = new Uri(this.configuration.ConnectionString)
            };

            this.connection = factory.CreateConnection();
            this.channel = this.connection.CreateModel();

            this.logger.LogInformation("Connected to RabbitMQ Broker");
            this.SetUpQos();
            this.RegisterQueues(controllerDescriptors);
        }

        private Task<object?> SendRoutedMessage(RabbitRoutedMessage message)
        {
            return Task.Run(() =>
            {
                var props = this.channel.CreateBasicProperties();
                props.CorrelationId = message.CorrelationId;
                props.Headers = message.Headers;
                props.Headers["MessageType"] = (int) message.MessageType;

                if (message.MessageType == MessageType.RpcRequest)
                {
                    this.responseAwaitations[message.CorrelationId] = new ResponseAwaitation
                    {
                        TaskCompletionSource = new TaskCompletionSource<object>(),
                        StartTicks = DateTime.Now.Ticks
                    };
                    props.ReplyTo = Utilities.AmqDirectReplyToQueue;
                }

                var routingKey = message.RoutingKey;
                var exchange = message.Exchange;

                var payloadString = Serialization.Serialize(message.Payload);
                var payloadType = message.Payload.GetType().FullName;
                var messageBytes = Encoding.UTF8.GetBytes(payloadString);

                props.Type = payloadType;

                this.channel.BasicPublish(exchange, routingKey, props, messageBytes);

                if (message.MessageType == MessageType.RpcRequest)
                {
                    var awaitation = this.responseAwaitations[message.CorrelationId];
                    var task = awaitation.TaskCompletionSource.Task;

                    task.Wait(Utilities.DefaultTimeout);
                    this.responseAwaitations.Remove(message.CorrelationId);

                    var duration = (DateTime.Now.Ticks - awaitation.StartTicks) / TimeSpan.TicksPerMillisecond;
                    if (!task.IsCompletedSuccessfully)
                    {
                        throw new TimeoutException(
                            $"Timeout after {duration}ms while waiting for a response when calling {message.RoutingKey}");
                    }


                    this.logger.LogInformation(
                        $"Successfully processed request to {message.RoutingKey} in {duration}ms");
                    return task;
                }

                return Task.FromResult<object>(null);
            });
        }

        private void SetUpQos()
        {
            this.logger.LogInformation("Setting Qos properties");
            this.channel.BasicQos(0, 1, false);
        }

        private void SetUpConsumer(string queue, bool autoAck = false)
        {
            this.logger.LogInformation($"Creating consumer for queue {queue}");
            var consumer = new EventingBasicConsumer(this.channel);
            consumer.Received += this.OnMessageReceived;
            this.channel.BasicConsume(queue, autoAck, consumer);
        }

        private void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            Task.Run(() =>
            {
                var payloadBytes = e.Body.ToArray();
                var properties = e.BasicProperties;
                var headers = properties.Headers;
                var messageType = (MessageType) headers["MessageType"];

                var payloadType = Serialization.GetTypeFromString(properties.Type);
                var payloadString = Encoding.UTF8.GetString(payloadBytes);
                var payload = Serialization.Deserialize(payloadString, payloadType);

                if (messageType == MessageType.RpcRequest)
                {
                    this.ProcessRpcRequest(properties, e.DeliveryTag, payload);
                }
                else if (messageType == MessageType.RpcResponse)
                {
                    this.responseAwaitations[properties.CorrelationId].TaskCompletionSource.SetResult(payload);
                }
            });
        }

        private async Task ProcessRpcRequest(IBasicProperties properties, ulong deliveryTag, object payload)
        {
            var stopwatch = Stopwatch.StartNew();
            var headers = properties.Headers;
            var controller = Encoding.UTF8.GetString(headers["Controller"] as byte[]);
            var procedure = Encoding.UTF8.GetString(headers["Procedure"] as byte[]);

            try
            {
                var result = this.messagingFacade.InvokeProcedure(controller, procedure, (object[]) payload);
                this.channel.BasicAck(deliveryTag, false);

                await this.SendRoutedMessage(new RabbitRoutedMessage
                {
                    Exchange = Utilities.AmqDirectExchange,
                    RoutingKey = properties.ReplyTo,
                    CorrelationId = properties.CorrelationId,
                    Payload = result,
                    MessageType = MessageType.RpcResponse
                }).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Failed to execute " + controller + ":" + procedure + "!");
                throw;
            }

            this.logger.LogInformation(
                $"Responded to {ProcedurePath.FromStrings(controller, procedure)} in {stopwatch.ElapsedMilliseconds} ms");
        }

        private void RegisterQueues(IEnumerable<ControllerDescriptor> descriptors)
        {
            foreach (var controllerDescriptor in descriptors)
            foreach (var procedureDescriptor in controllerDescriptor.ProcedureDescriptors)
            {
                var queueName = ProcedurePath.FromProcedureDescriptor(procedureDescriptor);
                this.channel.QueueDeclare(queueName, false, false);
                this.SetUpConsumer(queueName);
            }

            this.SetUpConsumer(Utilities.AmqDirectReplyToQueue, true);
        }
    }
}