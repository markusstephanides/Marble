using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Rabbit.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Marble.Messaging.Rabbit
{
    public class RabbitMessagingAdapter : IMessagingAdapter
    {
        public IObservable<RemoteMessage> MessageFeed { get; }

        private readonly RabbitConfiguration configuration;
        private readonly ISerializationAdapter serializationAdapter;
        private readonly ILogger<RabbitMessagingAdapter> logger;
        private readonly ISubject<RemoteMessage> messageFeedsSubject;

        private IModel channel;
        private IConnection connection;

        public RabbitMessagingAdapter(ILogger<RabbitMessagingAdapter> logger, IOptions<RabbitConfiguration> configuration,
            ISerializationAdapter serializationAdapter)
        {
            this.logger = logger;
            this.configuration = configuration.Value;
            this.serializationAdapter = serializationAdapter;
            this.messageFeedsSubject = new Subject<RemoteMessage>();
            this.MessageFeed = this.messageFeedsSubject;
        }

        public void Connect()
        {
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
            this.RegisterQueues();
        }

        public Task SendRemoteMessage(RemoteMessage remoteMessage)
        {
             return Task.Run(() =>
            {
                try
                {
                    var props = this.channel.CreateBasicProperties();
                    props.Headers = this.SerializeHeaderValues(remoteMessage.Headers);
                    props.ReplyTo = remoteMessage.ReplyTo ?? Utilities.AmqDirectReplyToQueue;
                    props.Type = remoteMessage.MessageType.ToString();

                    var routingKey = remoteMessage.Target;
                    var exchange = Utilities.AmqDirectExchange;

                    this.channel.BasicPublish(exchange, routingKey, props, remoteMessage.Payload);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }

        private void SetUpQos()
        {
            this.logger.LogInformation("Setting Qos properties");
            // TODO: Change prefetch count
            this.channel.BasicQos(0, 10, false);
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
            try
            {
                var properties = e.BasicProperties;
                var remoteMessage = new RemoteMessage
                {
                    Payload = e.Body.ToArray(),
                    Headers = this.DeserializeHeaderValues(properties.Headers),
                    ReplyTo = properties.ReplyTo,
                    MessageType = Enum.Parse<MessageType>(properties.Type)
                };
                // TODO is this really best practise? Maybe we want to have a Marble level ack-system
                if (remoteMessage.MessageType == MessageType.RequestMessage)
                {
                    this.channel.BasicAck(e.DeliveryTag, false);
                }
                
                this.messageFeedsSubject.OnNext(remoteMessage);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private IDictionary<string, object> SerializeHeaderValues(IDictionary<string, object> dictionary)
        {
            var resultingDictionary = new Dictionary<string, object>();
            
            foreach (var (key, value) in dictionary)
            {
                resultingDictionary[key] = this.serializationAdapter.Serialize(value);
            }

            return resultingDictionary;
        }

        private IDictionary<string, object> DeserializeHeaderValues(IDictionary<string, object> dictionary)
        {
            var resultingDictionary = new Dictionary<string, object>();
 
            foreach (var (key, value) in dictionary)
            {
                resultingDictionary[key] = this.serializationAdapter.Deserialize((byte[]) value, typeof(object));
            }

            return resultingDictionary;
        }


        private void RegisterQueues()
        {
            foreach (var procedurePath in this.configuration.KnownProcedurePaths)
            {
                this.channel.QueueDeclare(procedurePath, false, false);
                this.SetUpConsumer(procedurePath);
            }

            this.SetUpConsumer(Utilities.AmqDirectReplyToQueue, true);
        }
    }
}