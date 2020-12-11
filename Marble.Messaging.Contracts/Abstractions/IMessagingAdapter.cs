using System;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Models;

namespace Marble.Messaging.Contracts.Abstractions
{
    public interface IMessagingAdapter
    {
        IObservable<RemoteMessage> MessageFeed { get; }
        void Connect();
        Task SendRemoteMessage(RemoteMessage remoteMessage);
    }
}