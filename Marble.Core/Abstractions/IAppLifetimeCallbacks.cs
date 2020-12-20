using System;

namespace Marble.Core.Abstractions
{
    public interface IAppLifetimeCallbacks
    {
        void OnAppStarted(IServiceProvider serviceProvider)
        {
        }

        void OnAppStopping()
        {
        }
    }
}