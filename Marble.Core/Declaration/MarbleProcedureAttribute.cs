using System;

namespace Marble.Core.Declaration
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MarbleProcedureAttribute : Attribute
    {
        public string? Name { get; set; }
    }
}