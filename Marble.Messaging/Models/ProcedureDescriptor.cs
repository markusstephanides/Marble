﻿using System.Reflection;

namespace Marble.Messaging.Models
{
    public class ProcedureDescriptor
    {
        public string Name { get; set; }
        public MethodInfo MethodInfo { get; set; }
        public ControllerDescriptor ControllerDescriptor { get; set; }
    }
}