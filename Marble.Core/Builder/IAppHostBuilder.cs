using System;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Builder
{
    public interface IAppHostBuilder
    {
        IAppHostBuilder ConfigureServices(Action<IServiceCollection> configurationAction);
        IAppHostBuilder WithMessaging();
        IAppHostBuilder ProvideServiceCollection(IServiceCollection serviceCollection);
        IAppHostBuilder ProvideServiceProvider(IServiceProvider serviceProvider);
        AppHost BuildAndHost();
    }
}