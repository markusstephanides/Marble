using System;
using Marble.Core.Models;
using Marble.Messaging.Contracts.Configuration;

namespace Marble.Core.Hooks
{
    public interface IHookListener
    {
        // Messaging - Connection
        void OnBeforeMessagingAdapterConnect(Type messagingAdapterType, MessagingConfiguration messagingConfiguration)
        {
        }

        void OnMessagingAdapterConnected(Type messagingAdapterType, MessagingConfiguration messagingConfiguration)
        {
        }

        void OnMessagingAdapterConnectionLost()
        {
        }

        void OnMessagingAdapterReconnecting()
        {
        }

        void OnMessagingAdapterReconnected()
        {
        }

        void OnBeforeMessagingAdapterDisconnect(StopReason stopReason)
        {
        }

        void OnMessagingAdapterDisconnected(StopReason stopReason)
        {
        }

        // Messaging - Messages
        void OnRemoteMessageReceived()
        {
        }

        void OnRequestMessageReceived()
        {
        }

        void OnRemoteMessageSending()
        {
        }

        void OnRemoteMessageSent()
        {
        }
    }
}