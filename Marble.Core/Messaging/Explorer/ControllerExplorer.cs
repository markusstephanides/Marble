using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marble.Core.Declaration;

namespace Marble.Core.Messaging.Explorer
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
                        var descriptor = new ControllerDescriptor
                        {
                            ControllerName = type.FullName,
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
                .Select(method => new ProcedureDescriptor
                {
                    Name = method.Name,
                    MethodInfo = method,
                    ControllerDescriptor = controllerDescriptor
                });
        }
    }
}