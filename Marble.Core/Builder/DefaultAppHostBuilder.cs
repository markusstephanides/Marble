using System;
using Marble.Core.Messaging;
using Marble.Core.Messaging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Builder
{
    public class DefaultAppHostBuilder : IAppHostBuilder
    {
        private readonly AppHostBuildingModel buildingModel = new AppHostBuildingModel();

        public IAppHostBuilder ConfigureServices(Action<IServiceCollection> configurationAction)
        {
            this.buildingModel.ServiceCollectionConfigurationActions.Add(configurationAction);
            return this;
        }

        public IAppHostBuilder WithMessaging<TMessagingClient>() 
            where TMessagingClient : class, IMessagingClient
        {
            this.buildingModel.MessagingFacade = new MessagingFacade();
            this.ConfigureServices(this.buildingModel.MessagingFacade.ConfigureServices);
            this.ConfigureServices(services => services.AddSingleton<IMessagingClient, TMessagingClient>());
            return this;
        }

        public IAppHostBuilder ProvideServiceCollection(IServiceCollection serviceCollection)
        {
            this.buildingModel.ServiceCollection = serviceCollection;
            return this;
        }

        public IAppHostBuilder ProvideServiceProvider(IServiceProvider serviceProvider)
        {
            this.buildingModel.ServiceProvider = serviceProvider;
            return this;
        }

        public IAppHostBuilder ProvideConfiguration(IConfiguration configuration)
        {
            this.buildingModel.Configuration = configuration;
            return this;
        }

        public AppHost BuildAndHost()
        {
            return AppHostFactory.Create(this.buildingModel);
        }
    }
}