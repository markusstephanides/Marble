using System;

namespace Marble.Core.Abstractions
{
    public interface IServiceProviderAvailable
    {
        void OnServiceProviderAvailable(IServiceProvider serviceProvider);
    }
}