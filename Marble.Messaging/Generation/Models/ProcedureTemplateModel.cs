using System.Collections.Generic;
using DotLiquid;

namespace Marble.Messaging.Generation.Models
{
    public class ProcedureTemplateModel
    {
        public string MethodName { get; set; }
        public string MethodReturnType { get; set; }
        public string PureReturnType { get; set; }
        public bool IsGeneric { get; set; }
        public string CallingMethodName { get; set; }
        public List<Hash> Parameters { get; set; }
        public string ReturnTypeVariant { get; set; }
        public string Name { get; set; }

        public string ProcedureParametersModelTypeName { get; set; }
    }
}