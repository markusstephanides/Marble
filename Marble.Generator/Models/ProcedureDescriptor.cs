﻿using System.Collections.Generic;

namespace Marble.Generator.Models
{
    public class ProcedureDescriptor
    {
        public string Name { get; set; }
        public List<ProcedureArgument> Arguments { get; set; }
        public string ReturnType { get; set; }
    }
}