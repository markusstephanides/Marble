using System.Collections.Generic;

namespace Marble.Messaging.Generation.Models
{
    public class ProcedureTemplateModel
    {
        public string MethodName { get; set; }
        public string MethodReturnType { get; set; }
        public string GenericType { get; set; }
        public bool IsGeneric { get; set; }
        public string CallingMethodName { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}