using Microsoft.Extensions.DependencyInjection;

namespace Marble.Core.Abstractions
{
    public interface IServicesConfigurable
    {
        void ConfigureServices(IServiceCollection serviceCollection);
    }
}