using System;
using System.Collections.Generic;

namespace Marble.Messaging.Models
{
    public class ControllerDescriptor
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public IEnumerable<ProcedureDescriptor> ProcedureDescriptors { get; set; }
    }
}