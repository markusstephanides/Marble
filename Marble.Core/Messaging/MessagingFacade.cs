using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marble.Core.Declaration;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Explorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marble.Core.Messaging
{
    public class MessagingFacade
    {
        private ILogger<MessagingFacade>? logger;
        private IMessagingClient messagingClient;
        private IDictionary<ControllerDescriptor, object> controllers;

        public MessagingFacade()
        {
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            this.controllers = new ConcurrentDictionary<ControllerDescriptor, object>(
                new ControllerExplorer().ScanAssembly(Assembly.GetEntryAssembly()).Select(
                    controller => new KeyValuePair<ControllerDescriptor, object>(controller, null)
                )
            );

            foreach (var entry in this.controllers)
            {
                serviceCollection.AddSingleton(entry.Key.Type);
            }
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            this.messagingClient = serviceProvider.GetService<IMessagingClient>();
            this.logger = serviceProvider.GetService<ILogger<MessagingFacade>>();
            
            foreach (var (descriptor, _) in this.controllers)
            {
                this.controllers[descriptor] = serviceProvider.GetService(descriptor.Type);
                this.logger?.LogInformation($"Found controller instance for {descriptor.ControllerName}");
            }
        }
    }
}