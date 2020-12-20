using System;
using Marble.Core.Lifetime;

namespace Marble.Core.Models
{
    public class AppLifetime
    {
        public AppLifetime()
        {
            this.OnAppStarted = new LifetimeHandle<IServiceProvider>();
            this.OnAppStopping = new LifetimeHandle<StopReason>();
        }

        public LifetimeHandle<IServiceProvider> OnAppStarted { get; }
        public LifetimeHandle<StopReason> OnAppStopping { get; }
    }
}