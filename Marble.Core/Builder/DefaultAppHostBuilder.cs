using System;
using System.Linq;
using System.Threading;
using Marble.Core.Abstractions;
using Marble.Core.Hooks;
using Marble.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Marble.Core.Builder
{
    public class DefaultAppHostBuilder : IAppHostBuilderWithExposedModel
    {
        public DefaultAppHostBuilder()
        {
            this.RunPreBuildSteps();
        }

        public AppHostBuildingModel BuildingModel { get; } = new AppHostBuildingModel();

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

        public AppHost BuildAndHost()
        {
            return AppHostFactory.Create(this.BuildingModel);
        }

        public AppHost BuildAndHost<TEntryService>() where TEntryService : class, IEntryService
        {
            this.ConfigureServices(services => services.AddSingleton<IEntryService, TEntryService>());
            return this.BuildAndHost();
        }

        public AppHost BuildExternallyHosted(CancellationToken appStoppingCancellationToken)
        {
            this.BuildingModel.ShouldBeHostedExternally = true;
            this.BuildingModel.ProvidedCancellationToken = appStoppingCancellationToken;
            return AppHostFactory.Create(this.BuildingModel);
        }

        public IAppHostBuilder AddHookListener<THookListener>() where THookListener : IHookListener
        {
            throw new NotImplementedException();
        }

        private void RunPreBuildSteps()
        {
            Console.WriteLine("Starting...");

            this.ConfigureServices(collection =>
            {
                // Setup logging
                collection.AddLogging(loggingBuilder =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(this.BuildingModel.Configuration)
                        .CreateLogger();

                    const string sectionName = "Serilog";

                    if (this.BuildingModel.Configuration!.GetSection(sectionName).Exists())
                    {
                        Log.Logger = new LoggerConfiguration()
                            .ReadFrom.Configuration(this.BuildingModel.Configuration, sectionName)
                            .CreateLogger();
                    }
                    else
                    {
                        Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .CreateLogger();
                    }

                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog();
                });

                // Lifetime
                collection.AddSingleton(this.BuildingModel.AppLifetime);
            });
        }

        private void ConfigureLogging(bool serviceCollectionProvided = false)
        {
        }
    }
}