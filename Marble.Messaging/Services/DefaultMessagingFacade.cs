using System;
using Marble.Core.Serialization;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marble.Messaging.Services
{
    public class DefaultMessagingFacade<TMessagingAdapter, TMessagingConfiguration> : IMessagingFacade
        where TMessagingAdapter : class, IMessagingAdapter
        where TMessagingConfiguration : MessagingConfiguration
    {
        private readonly Type messagingConfigurationType;
        private readonly IControllerRegistry controllerRegistry;

        // Resolved when service provider is available
        private IMessagingAdapter messagingAdapter;
        private IMessageHandler messageHandler;
        private ILogger<DefaultMessagingFacade<TMessagingAdapter, TMessagingConfiguration>> logger;
        private MessagingConfiguration messagingConfiguration;

        public DefaultMessagingFacade(Type messagingConfigurationType)
        {
            this.messagingConfigurationType = messagingConfigurationType;
            this.controllerRegistry = new DefaultControllerRegistry();
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(_ => this.controllerRegistry);
            serviceCollection.Configure<TMessagingConfiguration>(configuration =>
            {
                configuration.KnownProcedurePaths = this.controllerRegistry.AvailableProcedurePaths;
            });
            // TODO: This is more like a workaround
            serviceCollection.AddSingleton<ISerializationAdapter>(provider =>
            {
                // TODO: Make this work
                // var configuration = provider.GetService<IOptions<TMessagingConfiguration>>();
                // return (ISerializationAdapter) Activator.CreateInstance(configuration.Value.SerializationAdapterType);
                return new DefaultJsonSerializationAdapter();
            });
            serviceCollection.AddSingleton<IMessagingAdapter, TMessagingAdapter>();
            serviceCollection.AddSingleton<IMessagingClient, DefaultMessagingClient>();
            serviceCollection.AddSingleton<IMessageHandler, DefaultMessageHandler>();
            serviceCollection.AddSingleton<IStreamManager, DefaultStreamManager>();

            this.controllerRegistry.ConfigureServices(serviceCollection);
        }

        public void OnServiceProviderAvailable(IServiceProvider serviceProvider)
        {
            this.controllerRegistry.OnServiceProviderAvailable(serviceProvider);
            
            this.logger = serviceProvider.GetService<ILogger<DefaultMessagingFacade<TMessagingAdapter, TMessagingConfiguration>>>();
            this.messagingConfiguration =
                (MessagingConfiguration) serviceProvider.GetService(this.messagingConfigurationType);
            this.messagingAdapter = serviceProvider.GetService<IMessagingAdapter>();
            this.messageHandler = serviceProvider.GetService<IMessageHandler>();
            
            this.messageHandler.Initialize();
            this.messagingAdapter.Connect();
        }
    }
}