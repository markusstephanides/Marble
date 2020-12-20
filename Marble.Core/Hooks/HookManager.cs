using System;
using System.Collections.Generic;
using Marble.Core.Models;
using Marble.Messaging.Contracts.Configuration;

namespace Marble.Core.Hooks
{
    public class HookManager : IHookListener
    {
        private readonly List<IHookListener> hookListeners;

        public HookManager()
        {
            this.hookListeners = new List<IHookListener>();
        }

        public void OnBeforeMessagingAdapterConnect(Type messagingAdapterType,
            MessagingConfiguration messagingConfiguration)
        {
            this.hookListeners.ForEach(listener =>
                listener.OnBeforeMessagingAdapterConnect(messagingAdapterType, messagingConfiguration));
        }

        public void OnMessagingAdapterConnected(Type messagingAdapterType,
            MessagingConfiguration messagingConfiguration)
        {
            this.hookListeners.ForEach(listener =>
                listener.OnMessagingAdapterConnected(messagingAdapterType, messagingConfiguration));
        }

        public void OnBeforeMessagingAdapterDisconnect(StopReason stopReason)
        {
            this.hookListeners.ForEach(listener => listener.OnBeforeMessagingAdapterDisconnect(stopReason));
        }

        public void OnMessagingAdapterDisconnected(StopReason stopReason)
        {
            this.hookListeners.ForEach(listener => listener.OnMessagingAdapterDisconnected(stopReason));
        }
    }
}