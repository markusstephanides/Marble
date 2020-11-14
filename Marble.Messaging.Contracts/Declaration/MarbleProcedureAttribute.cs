using System;

namespace Marble.Messaging.Contracts.Declaration
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MarbleProcedureAttribute : Attribute
    {
        public string? Name { get; set; }
    }
}