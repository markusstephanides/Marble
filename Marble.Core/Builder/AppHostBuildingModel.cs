using System;
using System.Collections.Generic;
using Marble.Core.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Builder
{
    public class AppHostBuildingModel
    {
        public IServiceProvider? ServiceProvider { get; set; }
        public IServiceCollection? ServiceCollection { get; set; }
        public IConfiguration? Configuration { get; set; }
        public MessagingFacade? MessagingFacade { get; set; }

        public IList<Action<IServiceCollection>> ServiceCollectionConfigurationActions { get; } =
            new List<Action<IServiceCollection>>();
    }
}