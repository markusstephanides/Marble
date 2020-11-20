using System;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Configuration;
using Marble.Messaging.Contracts.Models;

namespace Marble.Messaging.Contracts.Abstractions
{
    public interface IMessagingAdapter
    {
        void Connect(MessagingClientConfiguration configuration);
        IObservable<RemoteMessage> MessageFeed { get; }
        Task SendRemoteMessage(RemoteMessage remoteMessage);
    }
}