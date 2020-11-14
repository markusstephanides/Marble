using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marble.Messaging.Contracts.Declaration;
using Marble.Messaging.Models;
using Marble.Messaging.Transformers;

namespace Marble.Messaging.Explorer
{
    public class ControllerExplorer
    {
        public IEnumerable<ControllerDescriptor> ScanAssembly(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(type => Attribute.IsDefined(type, typeof(MarbleControllerAttribute)))
                .Select(
                    type =>
                    {
                        var controllerAttribute =
                            (MarbleControllerAttribute) type.GetCustomAttributes(typeof(MarbleControllerAttribute),
                                true)[0];
                        var descriptor = new ControllerDescriptor
                        {
                            Name = controllerAttribute.Name ?? ControllerName.FromType(type),
                            Type = type
                        };

                        descriptor.ProcedureDescriptors = this.ScanController(type, descriptor).ToList();
                        return descriptor;
                    });
        }

        private IEnumerable<ProcedureDescriptor> ScanController(Type type, ControllerDescriptor controllerDescriptor)
        {
            return type.GetMethods()
                .Where(method => method.GetCustomAttributes(typeof(MarbleProcedureAttribute), false).Any())
                .Select(method =>
                {
                    var controllerAttribute =
                        (MarbleControllerAttribute) type.GetCustomAttributes(typeof(MarbleControllerAttribute),
                            true)[0];
                    return new ProcedureDescriptor
                    {
                        Name = controllerAttribute.Name ?? ProcedureName.FromMethodInfo(method),
                        MethodInfo = method,
                        ControllerDescriptor = controllerDescriptor
                    };
                });
        }
    }
}