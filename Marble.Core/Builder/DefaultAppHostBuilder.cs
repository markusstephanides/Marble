using System;
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

        public IAppHostBuilder WithMessaging()
        {
            throw new NotImplementedException();
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