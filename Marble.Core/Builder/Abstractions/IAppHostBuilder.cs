using System;
using Marble.Core.Builder.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Builder.Abstractions
{
    public interface IAppHostBuilder
    {
        IAppHostBuilder ConfigureServices(Action<IServiceCollection> configurationAction);

        IAppHostBuilder Configure<TOption>(Func<IConfiguration, IConfiguration> configurationAction)
            where TOption : class;

        IAppHostBuilder Configure<TOption>(Action<TOption> optionConfigurationAction)
            where TOption : class;

        IAppHostBuilder ProvideServiceCollection(IServiceCollection serviceCollection);
        IAppHostBuilder ProvideServiceProvider(IServiceProvider serviceProvider);
        IAppHostBuilder ProvideConfiguration(IConfiguration configuration);
        AppHost BuildAndHost(bool keepRunning = true);
    }
}