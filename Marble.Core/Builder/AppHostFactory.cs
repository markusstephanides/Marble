﻿using System.IO;
using Marble.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Marble.Core.Builder
{
    public class AppHostFactory
    {
        public static AppHost Create(AppHostBuildingModel model)
        {
            SetupConfiguration(model);
            SetupLogging(model);
            ConfigureDependencyInjection(model);

            var appHost = new AppHost(model);

            appHost.Run();

            return appHost;
        }

        private static void SetupLogging(AppHostBuildingModel model)
        {
            const string sectionName = "Serilog";

            if (model.Configuration!.GetSection(sectionName).Exists())
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(model.Configuration, sectionName)
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();
            }
        }

        private static void SetupConfiguration(AppHostBuildingModel buildingModel)
        {
            if (buildingModel.Configuration != null)
            {
                return;
            }

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables();

            buildingModel.Configuration = configBuilder.Build();
        }

        private static void ConfigureDependencyInjection(AppHostBuildingModel buildingModel)
        {
            if (buildingModel.ServiceCollection == null)
            {
                buildingModel.ServiceCollection = new ServiceCollection();

                foreach (var configurationAction in buildingModel.ServiceCollectionConfigurationActions)
                {
                    configurationAction(buildingModel.ServiceCollection);
                }
            }

            buildingModel.ServiceProvider ??= buildingModel.ServiceCollection.BuildServiceProvider();
        }
    }
}