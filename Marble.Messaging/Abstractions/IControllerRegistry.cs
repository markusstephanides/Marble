using System.Collections.Generic;
using Marble.Core.Abstractions;
using Marble.Messaging.Contracts.Models;

namespace Marble.Messaging.Abstractions
{
    public interface IControllerRegistry : IServicesConfigurable, IAppLifetimeCallbacks
    {
        public List<string> AvailableProcedurePaths { get; set; }
        public MessageHandlingResult InvokeProcedure(string controllerName, string procedureName, object[]? parameters);
    }
}