using System.Collections.Generic;
using Marble.Core.Abstractions;
using Marble.Messaging.Contracts.Models.Message;
using Marble.Messaging.Contracts.Models.Message.Handling;

namespace Marble.Messaging.Abstractions
{
    public interface IControllerRegistry : IServicesConfigurable, IAppLifetimeCallbacks
    {
        public List<string> AvailableProcedurePaths { get; set; }

        public MessageHandlingResult InvokeProcedure(string controllerName, string procedureName,
            ParametersModel? parameters);
    }
}