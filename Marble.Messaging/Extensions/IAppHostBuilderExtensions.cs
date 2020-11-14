using Marble.Core.Builder;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Configuration;
using Marble.Messaging.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Messaging.Extensions
{
    public static class IAppHostBuilderExtensions
    {
        public static IAppHostBuilder WithMessaging<TMessagingClient, TConfiguration>(
            this IAppHostBuilder appHostBuilder, string configurationSection = "Messaging")
            where TMessagingClient : class, IMessagingClient
            where TConfiguration : MessagingClientConfiguration
        {
            var messagingFacade = new MessagingFacade();
            var builder = (IAppHostBuilderWithExposedModel) appHostBuilder;

            builder.BuildingModel.PostBuildActions.Add(model => { messagingFacade.Initialize(model.ServiceProvider); });

            return builder
                // Injection the required configuration
                .Configure<TConfiguration>(configuration => configuration.GetSection(configurationSection))
                .ConfigureServices(messagingFacade.ConfigureServices)
                .ConfigureServices(services => services.AddSingleton<IMessagingClient, TMessagingClient>());
        }
    }
}