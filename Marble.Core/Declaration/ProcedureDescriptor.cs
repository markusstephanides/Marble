using System.Reflection;

namespace Marble.Core.Declaration
{
    public class ProcedureDescriptor
    {
        public string Name { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public ControllerDescriptor ControllerDescriptor { get; set; }
    }
}