using System.IO;
using Marble.Core.Messaging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Builder
{
    public class AppHostFactory
    {
        public static AppHost Create(AppHostBuildingModel model)
        {
            SetupConfiguration(model);
            ConfigureDependencyInjection(model);
            InitializeMessaging(model);

            return new AppHost
            {
                ServiceProvider = model.ServiceProvider
            };
        }

        private static void InitializeMessaging(AppHostBuildingModel model)
        {
            if (model.MessagingFacade == null) return;
            
            model.MessagingFacade.Initialize(model.ServiceProvider);
            model.ServiceProvider.GetService<IMessagingClient>().Connect();
        }

        private static void SetupConfiguration(AppHostBuildingModel buildingModel)
        {
            if (buildingModel.Configuration != null) return;

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