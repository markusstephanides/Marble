using System.Threading.Tasks;
using Marble.Core.Messaging.Abstractions;
using Marble.Messaging.Rabbit.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marble.Messaging.Rabbit
{
    public class RabbitMessagingClient : IMessagingClient
    {
        private readonly ILogger<RabbitMessagingClient> logger;
        private readonly RabbitClientConfiguration configuration;

        public RabbitMessagingClient(IOptions<RabbitClientConfiguration> configurationOption, ILogger<RabbitMessagingClient> logger)
        {
            this.logger = logger;
            this.configuration = configurationOption.Value;
        }
        
        public Task<TResult> SendAsync<TResult>(RequestMessage requestMessage)
        {
            throw new System.NotImplementedException();
        }

        public Task SendAndForgetAsync(RequestMessage requestMessage)
        {
            throw new System.NotImplementedException();
        }

        public Task Reply<TResult>(string correlationId, TResult result)
        {
            throw new System.NotImplementedException();
        }

        public void Connect()
        {
            this.logger.LogInformation($"Connecting to {this.configuration.ConnectionString}");
        }
    }
}