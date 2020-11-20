using System;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marble.Messaging.Services
{
    public class DefaultMessagingFacade<T> : IMessagingFacade
    where T : IMessagingAdapter
    {
        private readonly IControllerRegistry controllerRegistry;
        private readonly IMessagingClient messagingClient;
        private readonly IMessagingAdapter messagingAdapter;
        private readonly IMessageHandler messageHandler;
        private readonly IStreamManager streamManager;
        private ISerializationAdapter serializationAdapter;
        private ILogger<DefaultMessagingFacade<T>> logger;

        public DefaultMessagingFacade()
        {
            this.controllerRegistry = new DefaultControllerRegistry();
            this.messagingAdapter = Activator.CreateInstance<T>();
            this.messagingClient = new DefaultMessagingClient(this.messagingAdapter, this.serializationAdapter, this.controllerRegistry);
            this.messageHandler = new DefaultMessageHandler(this.messagingAdapter, this.controllerRegistry, this.streamManager, this.serializationAdapter);
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(_ => this.messagingClient);
            this.controllerRegistry.ConfigureServices(serviceCollection);
        }

        public void OnServiceProviderAvailable(IServiceProvider serviceProvider)
        {
            this.logger = serviceProvider.GetService<ILogger<DefaultMessagingFacade<T>>>();
            this.controllerRegistry.OnServiceProviderAvailable(serviceProvider);
        }
    }
}