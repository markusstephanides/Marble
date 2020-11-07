using System.Text;
using Marble.Generator.Models;

namespace Marble.Generator.Templates
{
    public class ClientInterfaceTemplate
    {
        public static string Render(ControllerDescriptor descriptor, string linePrefix)
        {
            return $@"{linePrefix}public interface I{descriptor.Name} {{
{RenderMethods(descriptor, linePrefix + "\t")}
{linePrefix}}}";
        }

        private static string RenderMethods(ControllerDescriptor descriptor, string linePrefix)
        {
            var renderedString = new StringBuilder();

            foreach (var procedureDescriptor in descriptor.ProcedureDescriptors)
                renderedString.AppendLine(MethodTemplate.Render(descriptor, procedureDescriptor, linePrefix));

            return renderedString.ToString();
        }
    }
}