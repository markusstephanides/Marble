using System;
using System.IO;
using System.Linq;
using System.Threading;
using Marble.Core.Builder.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marble.Core.Builder
{
    public class AppHostFactory
    {
        public static AppHost Create(AppHostBuildingModel model)
        {
            RunPreBuildActions(model);
            SetupConfiguration(model);
            ConfigureDependencyInjection(model);
            RunPostBuildActions(model);
            StartBackgroundThread(model);

            var logger = model.ServiceProvider.GetService<ILogger<AppHostFactory>>();
            var elapsedTimeMs = (int)(DateTime.Now - model.CreationTime).TotalMilliseconds;
            logger.LogInformation($"Startup completed in {elapsedTimeMs} ms");
            
            return new AppHost
            {
                ServiceProvider = model.ServiceProvider
            };
        }

        private static void RunPostBuildActions(AppHostBuildingModel model)
        {
            model.PostBuildActions.ToList().ForEach(action => action(model));
        }

        private static void RunPreBuildActions(AppHostBuildingModel model)
        {
            model.PreBuildActions.ToList().ForEach(action => action(model));
        }

        private static void StartBackgroundThread(AppHostBuildingModel model)
        {
            if (!model.KeepRunning)
            {
                return;
            }

            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        private static void SetupConfiguration(AppHostBuildingModel buildingModel)
        {
            if (buildingModel.Configuration != null)
            {
                return;
            }

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true);

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