using System;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Builder
{
    public interface IAppHostBuilder
    {
        IAppHostBuilder ConfigureServices(Action<IServiceCollection> configurationAction);

        IAppHostBuilder WithMessaging<TMessagingClient, TConfiguration>(string configurationSection = "Messaging")
            where TMessagingClient : class, IMessagingClient
            where TConfiguration : MessagingClientConfiguration;
        IAppHostBuilder Configure<TOption>(Func<IConfiguration, IConfiguration> configurationAction)
            where TOption : class;
        IAppHostBuilder Configure<TOption>(Action<TOption> optionConfigurationAction)
            where TOption : class;
        IAppHostBuilder ProvideServiceCollection(IServiceCollection serviceCollection);
        IAppHostBuilder ProvideServiceProvider(IServiceProvider serviceProvider);
        IAppHostBuilder ProvideConfiguration(IConfiguration configuration);
        AppHost BuildAndHost();
    }
}