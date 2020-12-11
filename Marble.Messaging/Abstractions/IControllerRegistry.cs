using System;
using System.Collections.Generic;
using Marble.Core.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Models;

namespace Marble.Messaging.Abstractions
{
    public interface IControllerRegistry : IServicesConfigurable, IServiceProviderAvailable
    {
        public List<string> AvailableProcedurePaths { get; set; }
        public MessageHandlingResult InvokeProcedure(string controllerName, string procedureName, object[]? parameters);
    }
}