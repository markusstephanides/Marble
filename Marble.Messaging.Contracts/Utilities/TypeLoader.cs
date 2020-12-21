using System;

namespace Marble.Messaging.Utilities
{
    public class TypeLoader
    {
        public static Type FromString(string fullTypeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var t = assembly.GetType(fullTypeName, false);
                if (t != null)
                {
                    return t;
                }
            }

            throw new ArgumentException(
                "Type " + fullTypeName + " doesn't exist in the current app domain");
        }
    }
}