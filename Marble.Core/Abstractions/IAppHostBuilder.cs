using System;
using System.Threading;
using Marble.Core.Hooks;
using Marble.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Abstractions
{
    public interface IAppHostBuilder
    {
        IAppHostBuilder ConfigureServices(Action<IServiceCollection> configurationAction);

        IAppHostBuilder Configure<TOption>(Func<IConfiguration, IConfiguration> configurationAction)
            where TOption : class;

        IAppHostBuilder Configure<TOption>(Action<TOption> optionConfigurationAction)
            where TOption : class;

        IAppHostBuilder AddHookListener<TListener>() where TListener : IHookListener;

        IAppHostBuilder ProvideServiceCollection(IServiceCollection serviceCollection);
        IAppHostBuilder ProvideServiceProvider(IServiceProvider serviceProvider);
        IAppHostBuilder ProvideConfiguration(IConfiguration configuration);
        AppHost BuildAndHost();
        AppHost BuildAndHost<TEntryService>() where TEntryService : class, IEntryService;

        AppHost BuildExternallyHosted(CancellationToken appStoppingCancellationToken);
    }
}