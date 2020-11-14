using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Builder
{
    public class AppHostBuildingModel
    {
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public IServiceProvider? ServiceProvider { get; set; }
        public IServiceCollection? ServiceCollection { get; set; }
        public IConfiguration? Configuration { get; set; }

        public IList<Action<AppHostBuildingModel>> PreBuildActions { get; } = new List<Action<AppHostBuildingModel>>();

        public IList<Action<AppHostBuildingModel>> PostBuildActions { get; } = new List<Action<AppHostBuildingModel>>();

        public bool KeepRunning { get; set; }

        public IList<Action<IServiceCollection>> ServiceCollectionConfigurationActions { get; } =
            new List<Action<IServiceCollection>>();
    }
}