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
            if (!string.IsNullOrEmpty(accessModifier))
            {
                accessModifier += " ";
            }

            var shouldReceive = procedureDescriptor.ReturnType != null;
            var methodReturnType = !shouldReceive
                ? "Task"
                : $"Task<{procedureDescriptor.ReturnType}>";

            var methodTemplateBuilder =
                new StringBuilder(
                    $"{linePrefix}{accessModifier}{methodReturnType} {procedureDescriptor.MethodName}(");

            foreach (var argument in procedureDescriptor.Arguments)
            {
                methodTemplateBuilder.Append($"{argument.Type} {argument.Name}");
                if (argument != procedureDescriptor.Arguments.Last())
                {
                    methodTemplateBuilder.Append(", ");
                }
            }

            if (!withBody)
            {
                return methodTemplateBuilder.Append(");").ToString();
            }

            methodTemplateBuilder.AppendLine("){");
            methodTemplateBuilder.Append(linePrefix + "\t");

            var parameterString = new StringBuilder(procedureDescriptor.Arguments.Any() ? ", " : "");

            foreach (var argument in procedureDescriptor.Arguments)
            {
                parameterString.Append(argument.Name);
                if (procedureDescriptor.Arguments.Last() != argument)
                {
                    parameterString.Append(", ");
                }
            }

            var methodName = shouldReceive ? $"SendAsync<{procedureDescriptor.ReturnType}>" : "SendAndForgetAsync";

            var messagingClientCall =
                $"return this.messagingClient.{methodName}(new RequestMessage(\"{controllerDescriptor.Name}\", \"{procedureDescriptor.Name}\"{parameterString}));";

            methodTemplateBuilder.AppendLine(messagingClientCall);

            return methodTemplateBuilder.AppendLine(linePrefix + "}").ToString();
        }
    }
}