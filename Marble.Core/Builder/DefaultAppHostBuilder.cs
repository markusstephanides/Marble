using System;
using System.Linq;
using Marble.Core.Builder.Abstractions;
using Marble.Core.Builder.Models;
using Marble.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Marble.Core.Builder
{
    public class DefaultAppHostBuilder : IAppHostBuilderWithExposedModel
    {
        public AppHostBuildingModel BuildingModel { get; } = new AppHostBuildingModel();

        public DefaultAppHostBuilder()
        {
            // TODO: Move this
            this.ConfigureServices(collection =>
            {
                collection.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(LogLevel.Debug);
                    loggingBuilder.AddNLog(DefaultConfigurations.ConsoleTargetConfiguration);
                });
            });
        }

        public IAppHostBuilder ConfigureServices(Action<IServiceCollection> configurationAction)
        {
            this.BuildingModel.ServiceCollectionConfigurationActions.Add(configurationAction);
            return this;
        }

        public IAppHostBuilder Configure<TOption>(Func<IConfiguration, IConfiguration> configurationAction)
            where TOption : class
        {
            this.ConfigureServices(collection =>
            {
                collection.Configure<TOption>(configurationAction(this.BuildingModel.Configuration));
            });
            return this;
        }

        public IAppHostBuilder Configure<TOption>(Action<TOption> optionConfigurationAction) where TOption : class
        {
            this.ConfigureServices(collection => { collection.Configure(optionConfigurationAction); });
            return this;
        }

        public IAppHostBuilder AddClients()
        {
            throw new NotImplementedException();
        }

        public IAppHostBuilder ProvideServiceCollection(IServiceCollection serviceCollection)
        {
            this.BuildingModel.ServiceCollection = serviceCollection;
            this.BuildingModel.ServiceCollectionConfigurationActions.ToList()
                .ForEach(action => action(this.BuildingModel.ServiceCollection));
            return this;
        }

        public IAppHostBuilder ProvideServiceProvider(IServiceProvider serviceProvider)
        {
            this.BuildingModel.ServiceProvider = serviceProvider;
            return this;
        }

        public IAppHostBuilder ProvideConfiguration(IConfiguration configuration)
        {
            this.BuildingModel.Configuration = configuration;
            return this;
        }

        public AppHost BuildAndHost(bool keepRunning = true)
        {
            this.BuildingModel.KeepRunning = keepRunning;
            return AppHostFactory.Create(this.BuildingModel);
        }
    }
}