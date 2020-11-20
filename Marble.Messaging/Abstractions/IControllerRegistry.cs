using Marble.Core.Abstractions;

namespace Marble.Messaging.Abstractions
{
    public interface IControllerRegistry : IServicesConfigurable, IServiceProviderAvailable
    {
        public object? InvokeProcedure(string controllerName, string procedureName, object[]? parameters);
    }
}