using System;
using System.Collections.Generic;
using System.Threading;
using Marble.Core.Hooks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Models
{
    public class AppHostBuildingModel
    {
        public DateTime CreationTime { get; } = DateTime.Now;
        public IServiceProvider? ServiceProvider { get; set; }
        public IServiceCollection? ServiceCollection { get; set; }
        public IConfiguration? Configuration { get; set; }

        public HookManager HookManager { get; set; } = new HookManager();

        public AppLifetime AppLifetime { get; } = new AppLifetime();

        public IList<Action<AppHostBuildingModel>> PreBuildActions { get; } = new List<Action<AppHostBuildingModel>>();

        public IList<Action<AppHostBuildingModel>> PostBuildActions { get; } = new List<Action<AppHostBuildingModel>>();

        public bool ShouldBeHostedExternally { get; set; }

        public CancellationToken? ProvidedCancellationToken { get; set; }

        public IList<Action<IServiceCollection>> ServiceCollectionConfigurationActions { get; } =
            new List<Action<IServiceCollection>>();
    }
}