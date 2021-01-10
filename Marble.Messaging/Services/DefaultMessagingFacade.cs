using System;
using System.Collections.Generic;
using System.Reflection;
using Marble.Core.Hooks;
using Marble.Core.Models;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Configuration;
using Marble.Messaging.Explorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marble.Messaging.Services
{
    public class DefaultMessagingFacade<TMessagingAdapter, TMessagingConfiguration> : IMessagingFacade
        where TMessagingAdapter : class, IMessagingAdapter
        where TMessagingConfiguration : MessagingConfiguration
    {
        private readonly ClientExplorer clientExplorer;
        private readonly IControllerRegistry controllerRegistry;
        private readonly HookManager hookManager;
        private readonly Type messagingConfigurationType;
        private ILogger<DefaultMessagingFacade<TMessagingAdapter, TMessagingConfiguration>> logger;
        private IMessageHandler messageHandler;

        // Resolved when service provider is available
        private IMessagingAdapter messagingAdapter;
        private MessagingConfiguration messagingConfiguration;

        public DefaultMessagingFacade(Type messagingConfigurationType, HookManager hookManager,
            List<Assembly>? additionalAssemblies)
        {
            this.messagingConfigurationType = messagingConfigurationType;
            this.hookManager = hookManager;
            this.controllerRegistry = new DefaultControllerRegistry<TMessagingConfiguration>(additionalAssemblies);
            this.clientExplorer = new ClientExplorer();
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(_ => this.controllerRegistry);
            serviceCollection.Configure<TMessagingConfiguration>(configuration =>
                configuration.KnownProcedurePaths = this.controllerRegistry.AvailableProcedurePaths);
            // TODO: This looks more like a workaround
            serviceCollection.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptions<TMessagingConfiguration>>().Value;
                return (ISerializationAdapter) Activator.CreateInstance(config.SerializationAdapterType);
            });
            serviceCollection.AddSingleton<IMessagingAdapter, TMessagingAdapter>();
            serviceCollection.AddSingleton<IMessagingClient, DefaultMessagingClient<TMessagingConfiguration>>();
            serviceCollection.AddSingleton<IMessageHandler, DefaultMessageHandler>();
            serviceCollection.AddSingleton<IStreamManager, DefaultStreamManager>();

            this.clientExplorer.ConfigureServices(serviceCollection);
            this.controllerRegistry.ConfigureServices(serviceCollection);
        }

        public void OnAppStarted(IServiceProvider serviceProvider)
        {
            this.controllerRegistry.OnAppStarted(serviceProvider);

            this.logger = serviceProvider
                .GetService<ILogger<DefaultMessagingFacade<TMessagingAdapter, TMessagingConfiguration>>>()!;
            this.messagingConfiguration =
                (MessagingConfiguration) serviceProvider.GetService(this.messagingConfigurationType);
            this.messagingAdapter = serviceProvider.GetService<IMessagingAdapter>()!;
            this.messageHandler = serviceProvider.GetService<IMessageHandler>()!;

            this.messageHandler.Initialize();
            this.hookManager.OnBeforeMessagingAdapterConnect(typeof(TMessagingAdapter), this.messagingConfiguration);
            this.messagingAdapter.Connect();
            this.hookManager.OnMessagingAdapterConnected(typeof(TMessagingAdapter), this.messagingConfiguration);
        }

        public void OnAppStopping(StopReason stopReason)
        {
            this.hookManager.OnBeforeMessagingAdapterDisconnect(stopReason);
            this.messagingAdapter.Destroy();
            this.hookManager.OnMessagingAdapterDisconnected(stopReason);
        }
    }
}