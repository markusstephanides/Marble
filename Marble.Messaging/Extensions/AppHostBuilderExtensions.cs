using Marble.Core.Builder.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Configuration;
using Marble.Messaging.Services;
using Marble.Messaging.Utilities;

namespace Marble.Messaging.Extensions
{
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder WithMessaging<TMessagingClient, TConfiguration>(
            this IAppHostBuilder appHostBuilder, string configurationSection = "Messaging", TConfiguration defaultConfiguration = null)
            where TMessagingClient : class, IMessagingAdapter
            where TConfiguration : MessagingConfiguration
        {
            var messagingFacade = new DefaultMessagingFacade<TMessagingClient, TConfiguration>(typeof(TConfiguration));
            var builder = (IAppHostBuilderWithExposedModel) appHostBuilder;

            builder.BuildingModel.PostBuildActions.Add(model => { messagingFacade.OnServiceProviderAvailable(model.ServiceProvider); });

            return builder
                .Configure<TConfiguration>(configuration => configuration.GetSection(configurationSection))
                .Configure<TConfiguration>(configuration => ConfigurationMerger.Merge(configuration, defaultConfiguration))
                .ConfigureServices(messagingFacade.ConfigureServices);
        }
    }
}