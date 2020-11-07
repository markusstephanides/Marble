using System.Linq;
using System.Text;
using Marble.Generator.Models;

namespace Marble.Generator.Templates
{
    public class MethodTemplate
    {
        public static string Render(ControllerDescriptor controllerDescriptor, ProcedureDescriptor procedureDescriptor,
            string linePrefix,
            string accessModifier = "", bool withBody = false)
        {
            if (!string.IsNullOrEmpty(accessModifier)) accessModifier += " ";

            var methodTemplateBuilder =
                new StringBuilder(
                    $"{linePrefix}{accessModifier}Task<{procedureDescriptor.ReturnType}> {procedureDescriptor.Name}(");

            foreach (var argument in procedureDescriptor.Arguments)
            {
                methodTemplateBuilder.Append($"{argument.Type} {argument.Name}");
                if (argument != procedureDescriptor.Arguments.Last()) methodTemplateBuilder.Append(", ");
            }

            if (!withBody) return methodTemplateBuilder.Append(");").ToString();

            methodTemplateBuilder.AppendLine("){");
            methodTemplateBuilder.Append(linePrefix + "\t");

            var parameterString = new StringBuilder(procedureDescriptor.Arguments.Any() ? ", " : "");

            foreach (var argument in procedureDescriptor.Arguments)
            {
                parameterString.Append(argument.Name);
                if (procedureDescriptor.Arguments.Last() != argument) parameterString.Append(", ");
            }

            var messagingClientCall =
                $"return this.messagingClient.SendAsync<{procedureDescriptor.ReturnType}>(new RequestMessage(\"{controllerDescriptor.Name}\", \"{procedureDescriptor.Name}\"{parameterString}));";

            methodTemplateBuilder.AppendLine(messagingClientCall);

            return methodTemplateBuilder.AppendLine(linePrefix + "}").ToString();
        }
    }
}