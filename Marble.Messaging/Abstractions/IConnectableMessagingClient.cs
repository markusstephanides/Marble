using System.Collections.Generic;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Models;
using Marble.Messaging.Services;

namespace Marble.Messaging.Abstractions
{
    public interface IConnectableMessagingClient : IMessagingClient
    {
        void Connect(MessagingFacade messagingFacade, IEnumerable<ControllerDescriptor> controllers);
    }
}