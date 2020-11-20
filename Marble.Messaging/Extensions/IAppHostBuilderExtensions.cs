using Marble.Core.Builder;
using Marble.Core.Builder.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Configuration;
using Marble.Messaging.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Messaging.Extensions
{
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder WithMessaging<TMessagingClient, TConfiguration>(
            this IAppHostBuilder appHostBuilder, string configurationSection = "Messaging")
            where TMessagingClient : class, IMessagingAdapter
            where TConfiguration : MessagingClientConfiguration
        {
            var messagingFacade = new DefaultMessagingFacade<TMessagingClient>();
            var builder = (IAppHostBuilderWithExposedModel) appHostBuilder;

            builder.BuildingModel.PostBuildActions.Add(model => { messagingFacade.OnServiceProviderAvailable(model.ServiceProvider); });

            return builder
                .Configure<TConfiguration>(configuration => configuration.GetSection(configurationSection))
                .ConfigureServices(messagingFacade.ConfigureServices);
        }
    }
}