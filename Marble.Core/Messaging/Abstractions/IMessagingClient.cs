using System.Collections.Generic;
using System.Threading.Tasks;
using Marble.Core.Declaration;
using Marble.Core.Messaging.Models;

namespace Marble.Core.Messaging.Abstractions
{
    public interface IMessagingClient
    {
        Task<TResult> SendAsync<TResult>(RequestMessage requestMessage);
        Task SendAndForgetAsync(RequestMessage requestMessage);
        void Connect(MessagingFacade messagingFacade, IEnumerable<ControllerDescriptor> controllers);
    }
}