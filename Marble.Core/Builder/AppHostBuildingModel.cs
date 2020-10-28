using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Builder
{
    public class AppHostBuildingModel
    {
        public IServiceProvider ServiceProvider { get; set; }
        public IServiceCollection ServiceCollection { get; set; }
        public IList<Action<IServiceCollection>> ServiceCollectionConfigurationActions { get; set; } = new List<Action<IServiceCollection>>();
    }
}