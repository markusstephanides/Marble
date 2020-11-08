using Marble.Core.Declaration;
using Marble.Core.Messaging.Models;

namespace Marble.Core.Transformers
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