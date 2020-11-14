using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Models;

namespace Marble.Messaging.Transformers
{
    public class ProcedurePath
    {
        public static string FromProcedureDescriptor(ProcedureDescriptor procedureDescriptor)
        {
            return FromStrings(procedureDescriptor.ControllerDescriptor.Name,
                procedureDescriptor.Name);
        }

        public static string FromRequestMessage(RequestMessage requestMessage)
        {
            return FromStrings(requestMessage.Controller, requestMessage.Procedure);
        }

        public static string FromStrings(string controllerName, string procedureName)
        {
            return $"{controllerName}:{procedureName}";
        }
    }
}