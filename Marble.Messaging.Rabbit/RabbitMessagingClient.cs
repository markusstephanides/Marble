using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Marble.Core.Declaration;
using Marble.Core.Messaging;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Models;
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
            responseAwaitations = new Dictionary<string, ResponseAwaitation>();
            configuration = configurationOption.Value;
        }

        public async Task<TResult> SendAsync<TResult>(RequestMessage requestMessage)
        {
            return (TResult) await SendRoutedMessage(new RabbitRoutedMessage
            {
                Exchange = Utilities.AmqDirectExchange,
                MessageType = MessageType.RpcRequest,
                RoutingKey = requestMessage.ResolveQueueName(),
                CorrelationId = Guid.NewGuid().ToString(),
                Headers = new Dictionary<string, object>
                {
                    {"Controller", requestMessage.Controller},
                    {"Procedure", requestMessage.Procedure}
                },
                Payload = requestMessage.Arguments
            });
        }

        public Task SendAndForgetAsync(RequestMessage requestMessage)
        {
            return SendRoutedMessage(new RabbitRoutedMessage
            {
                Exchange = Utilities.AmqDirectExchange,
                RoutingKey = requestMessage.ResolveQueueName(),
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
            logger.LogInformation($"Connecting to {configuration.ConnectionString}");
            if (connection != null) throw new InvalidOperationException("Client is already connected!");

            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration.ConnectionString)
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            logger.LogInformation("Connected to RabbitMQ Broker");
            SetUpQos();
            RegisterQueues(controllerDescriptors);
        }

        private Task<object?> SendRoutedMessage(RabbitRoutedMessage message)
        {
            return Task.Run(() =>
            {
                var props = channel.CreateBasicProperties();
                props.CorrelationId = message.CorrelationId;
                props.Headers = message.Headers;
                props.Headers["MessageType"] = (int) message.MessageType;

                if (message.MessageType == MessageType.RpcRequest)
                {
                    responseAwaitations[message.CorrelationId] = new ResponseAwaitation
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

                channel.BasicPublish(exchange, routingKey, props, messageBytes);

                if (message.MessageType == MessageType.RpcRequest)
                {
                    var awaitation = responseAwaitations[message.CorrelationId];
                    var task = awaitation.TaskCompletionSource.Task;
                    task.Wait(Utilities.DefaultTimeout);

                    responseAwaitations.Remove(message.CorrelationId);
                    var duration = (DateTime.Now.Ticks - awaitation.StartTicks) / TimeSpan.TicksPerMillisecond;
                    if (!task.IsCompletedSuccessfully)
                        throw new TimeoutException(
                            $"Timeout after {duration}ms while waiting for a response when calling {message.RoutingKey}");

                    logger.LogInformation(
                        $"Successfully processed request to {message.RoutingKey} in {duration}ms");
                    return task;
                }

                return Task.FromResult<object>(null);
            });
        }

        private void SetUpQos()
        {
            logger.LogInformation("Setting Qos properties");
            channel.BasicQos(0, 1, false);
        }

        private void SetUpConsumer(string queue, bool autoAck = false)
        {
            logger.LogInformation($"Creating consumer for queue {queue}");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;
            channel.BasicConsume(queue, autoAck, consumer);
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
                    ProcessRpcRequest(properties, e.DeliveryTag, payload);
                else if (messageType == MessageType.RpcResponse)
                    responseAwaitations[properties.CorrelationId].TaskCompletionSource.SetResult(payload);
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
                var result = messagingFacade.InvokeProcedure(controller, procedure, (object[]) payload);

                channel.BasicAck(deliveryTag, false);

                await SendRoutedMessage(new RabbitRoutedMessage
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
                logger.LogError(exception, "Failed to execute " + controller + ":" + procedure + "!");
                throw;
            }

            logger.LogInformation(
                $"Responded to {controller}:{procedure} in {stopwatch.ElapsedMilliseconds} ms");
        }

        private void RegisterQueues(IEnumerable<ControllerDescriptor> descriptors)
        {
            foreach (var controllerDescriptor in descriptors)
            foreach (var procedureDescriptor in controllerDescriptor.ProcedureDescriptors)
            {
                var queueName = procedureDescriptor.ToString();
                channel.QueueDeclare(queueName, false, false);
                SetUpConsumer(queueName);
            }

            SetUpConsumer(Utilities.AmqDirectReplyToQueue, true);
        }
    }
}