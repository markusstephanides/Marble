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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RPT.MicroMan.Utilities;

namespace Marble.Messaging.Rabbit
{
    public class RabbitMessagingClient : IMessagingClient
    {
        private IConnection connection;
        private IModel channel;

        private readonly ILogger<RabbitMessagingClient> logger;
        private readonly RabbitClientConfiguration configuration;
        private readonly IDictionary<string, MessageMetaData> metaData;
        private readonly IDictionary<string, TaskCompletionSource<object>> rpcCompletionSources;
        private MessagingFacade messagingFacade;

        public RabbitMessagingClient(IOptions<RabbitClientConfiguration> configurationOption,
            ILogger<RabbitMessagingClient> logger)
        {
            this.logger = logger;
            this.metaData = new Dictionary<string, MessageMetaData>();
            this.rpcCompletionSources = new Dictionary<string, TaskCompletionSource<object>>();
            this.configuration = configurationOption.Value;
        }

        public async Task<TResult> SendAsync<TResult>(RequestMessage requestMessage)
        {
            return (TResult) await this.SendRoutedMessage(new RabbitRoutedMessage
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
                Payload = requestMessage.Arguments,
            });
        }

        public Task SendAndForgetAsync(RequestMessage requestMessage)
        {
            return this.SendRoutedMessage(new RabbitRoutedMessage
            {
                Exchange = Utilities.AmqDirectExchange,
                RoutingKey = requestMessage.ResolveQueueName(),
                Headers = new Dictionary<string, object>()
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
            this.channel = connection.CreateModel();

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
                    this.metaData[message.CorrelationId] = new MessageMetaData
                    {
                        CorrelationId = message.CorrelationId,
                        ReplyToQueue = Utilities.AmqDirectReplyToQueue
                    };
                
                    this.rpcCompletionSources[message.CorrelationId] = new TaskCompletionSource<object>();

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
                    var task = this.rpcCompletionSources[message.CorrelationId].Task;
                    task.Wait(Utilities.DefaultTimeout);
                    
                    if (task.IsCompletedSuccessfully)
                    {
                        return task;
                    }
                    
                    throw new TimeoutException("Timeout while waiting for a response when calling " + message.RoutingKey);
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
                    this.rpcCompletionSources[properties.CorrelationId].SetResult(payload);
                }
            });
        }

        private async Task ProcessRpcRequest(IBasicProperties properties, ulong deliveryTag, object payload)
        {
            var stopwatch = Stopwatch.StartNew();
            var headers = properties.Headers;
            var controller = Encoding.UTF8.GetString(headers["Controller"] as byte[]);
            var procedure = Encoding.UTF8.GetString(headers["Procedure"] as byte[]);
                    
            var result = await this.messagingFacade.InvokeProcedure(controller, procedure, (object[]) payload)
                .ConfigureAwait(false);

            this.channel.BasicAck(deliveryTag, false);

            try
            {
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
                $"Responded to {controller}:{procedure} in {stopwatch.ElapsedMilliseconds} ms");
        }

        private void RegisterQueues(IEnumerable<ControllerDescriptor> descriptors)
        {
            foreach (var controllerDescriptor in descriptors)
            {
                foreach (var procedureDescriptor in controllerDescriptor.ProcedureDescriptors)
                {
                    var queueName = procedureDescriptor.ToString();
                    this.channel.QueueDeclare(queueName, false, false);
                    this.SetUpConsumer(queueName, false);
                }
            }

            this.logger.LogInformation("Declared RPC queues");
            this.SetUpConsumer(Utilities.AmqDirectReplyToQueue, true);
            this.logger.LogInformation("Declared RPC reply queue");
        }
    }
}