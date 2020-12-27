using System.Collections.Generic;
using System.Reflection;
using Marble.Core.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Configuration;
using Marble.Messaging.Services;
using Marble.Messaging.Utilities;

namespace Marble.Messaging.Extensions
{
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder WithMessaging<TMessagingClient, TConfiguration>(
            this IAppHostBuilder appHostBuilder, string configurationSection = "Messaging",
            TConfiguration defaultConfiguration = null, List<Assembly>? additionalAssemblies = null)
            where TMessagingClient : class, IMessagingAdapter
            where TConfiguration : MessagingConfiguration
        {
            var builder = (IAppHostBuilderWithExposedModel) appHostBuilder;
            var messagingFacade =
                new DefaultMessagingFacade<TMessagingClient, TConfiguration>(typeof(TConfiguration),
                    builder.BuildingModel.HookManager, additionalAssemblies);

            builder.BuildingModel.AppLifetime.OnAppStarted.Register(messagingFacade.OnAppStarted);
            builder.BuildingModel.AppLifetime.OnAppStopping.Register(messagingFacade.OnAppStopping);

            return builder
                .Configure<TConfiguration>(configuration => configuration.GetSection(configurationSection))
                .Configure<TConfiguration>(configuration =>
                {
                    ConfigurationMerger.Merge(configuration, defaultConfiguration);
                })
                .ConfigureServices(messagingFacade.ConfigureServices);
        }
    }
}