using System.Text;
using Marble.Generator.Models;

namespace Marble.Generator.Templates
{
    public class ClientImplementationTemplate
    {
        public static string Render(ControllerDescriptor descriptor, string linePrefix)
        {
            var className = $"Default{descriptor.ClassName}Client";

            return $@"{linePrefix}public class {className} : I{descriptor.ClassName} {{
{linePrefix}{linePrefix}private readonly IMessagingClient messagingClient;

{linePrefix}{linePrefix}public {className} (IMessagingClient messagingClient) {{
{linePrefix}{linePrefix}{linePrefix}this.messagingClient = messagingClient;
{linePrefix}{linePrefix}}}

{RenderMethods(descriptor, linePrefix + "\t")}
{linePrefix}}}";
        }

        private static string RenderMethods(ControllerDescriptor descriptor, string linePrefix)
        {
            var renderedString = new StringBuilder();

            foreach (var procedureDescriptor in descriptor.ProcedureDescriptors)
            {
                renderedString.AppendLine(MethodTemplate.Render(descriptor, procedureDescriptor, linePrefix, "public",
                    true));
            }

            return renderedString.ToString();
        }
    }
}