using System.Collections.Generic;

namespace Marble.Generator.Models
{
    public class ControllerDescriptor
    {
        public string Name { get; set; }
        public IEnumerable<ProcedureDescriptor> ProcedureDescriptors { get; set; }
        public string ClassName { get; set; }
    }
}