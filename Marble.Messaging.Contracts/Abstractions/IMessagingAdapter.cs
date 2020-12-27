using System;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Models.Message;

namespace Marble.Messaging.Contracts.Abstractions
{
    public interface IMessagingAdapter
    {
        IObservable<RemoteMessage> MessageFeed { get; }
        void Connect();
        void Destroy();
        Task SendRemoteMessage(RemoteMessage remoteMessage);
    }
}