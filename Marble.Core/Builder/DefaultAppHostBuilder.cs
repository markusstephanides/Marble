using System;
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

        public AppHost BuildAndHost()
        {
            throw new NotImplementedException();
        }
    }
}