using Marble.Generator.Models;

namespace Marble.Generator.Templates
{
    public class ClientFileTemplate
    {
        public static string Render(TargetInformation targetInformation, ControllerDescriptor controllerDescriptor)
        {
            return
                $@"using System.Threading.Tasks;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Models;

namespace {targetInformation.Namespace} {{
{ClientInterfaceTemplate.Render(controllerDescriptor, "\t")}
{ClientImplementationTemplate.Render(controllerDescriptor, "\t")}
}}";
        }
    }
}