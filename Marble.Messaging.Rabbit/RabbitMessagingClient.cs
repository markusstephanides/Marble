using System;
using System.Threading.Tasks;
using Marble.Core.Messaging.Abstractions;

namespace Marble.Messaging.Rabbit
{
    public class RabbitMessagingClient : IMessagingClient
    {
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
            
        }
    }
}