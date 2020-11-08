using System;

namespace Marble.Core.Transformers
{
    public class ControllerName
    {
        public static string FromType(Type type)
        {
            return FromClassName(type.FullName);
        }

        public static string FromClassName(string className)
        {
            return className;
        }
    }
}