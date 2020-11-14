using System;

namespace Marble.Messaging.Contracts.Declaration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MarbleControllerAttribute : Attribute
    {
        public string? Name { get; set; }
    }
}