using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marble.Core.Declaration;

namespace Marble.Core.Messaging.Explorer
{
    public class ControllerExplorer
    {
        public IEnumerable<ControllerDescriptor> Scan(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(type => Attribute.IsDefined(type, typeof(MarbleController)))
                .Select(
                    type => new ControllerDescriptor
                    {
                        ControllerName = type.FullName,
                        Type = type,
                        ProcedureDescriptors = this.ScanController(type)
                    });
        }

        private IEnumerable<ProcedureDescriptor> ScanController(Type type)
        {
            return type.GetMethods().Select(method => new ProcedureDescriptor
            {
                Name = method.Name,
                MethodInfo = method
            });
        }
    }
}