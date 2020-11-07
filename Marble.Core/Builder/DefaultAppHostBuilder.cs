using System;
using System.Linq;
using System.Threading;
using Marble.Core.Messaging;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Models;
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

        public IAppHostBuilder WithMessaging<TMessagingClient, TConfiguration>(
            string configurationSection = "Messaging")
            where TMessagingClient : class, IMessagingClient
            where TConfiguration : MessagingClientConfiguration
        {
            this.buildingModel.MessagingFacade = new MessagingFacade();
            this.Configure<TConfiguration>(configuration => configuration.GetSection(configurationSection));
            this.ConfigureServices(this.buildingModel.MessagingFacade.ConfigureServices);
            this.ConfigureServices(services => services.AddSingleton<IMessagingClient, TMessagingClient>());
            return this;
        }

        public IAppHostBuilder Configure<TOption>(Func<IConfiguration, IConfiguration> configurationAction)
            where TOption : class
        {
            this.ConfigureServices(collection =>
            {
                collection.Configure<TOption>(configurationAction(this.buildingModel.Configuration));
            });
            return this;
        }

        public IAppHostBuilder Configure<TOption>(Action<TOption> optionConfigurationAction) where TOption : class
        {
            this.ConfigureServices(collection => { collection.Configure(optionConfigurationAction); });
            return this;
        }

        public IAppHostBuilder ProvideServiceCollection(IServiceCollection serviceCollection)
        {
            this.buildingModel.ServiceCollection = serviceCollection;
            this.buildingModel.ServiceCollectionConfigurationActions.ToList()
                .ForEach(action => action(this.buildingModel.ServiceCollection));
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

        public IAppHostBuilder KeepRunning()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }).Start();

            return this;
        }
    }
}