using System.Collections.Generic;
using Marble.Core.Abstractions;

namespace Marble.Messaging.Abstractions
{
    public interface IControllerRegistry : IServicesConfigurable, IServiceProviderAvailable
    {
        public List<string> AvailableProcedurePaths { get; set; }
        public object? InvokeProcedure(string controllerName, string procedureName, object[]? parameters);
    }
}