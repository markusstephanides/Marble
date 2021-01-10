using System;
using System.Collections.Generic;

namespace Marble.Messaging.Contracts.Utilities
{
    public class TypeLoader
    {
        private static readonly Dictionary<string, Type> Cache = new Dictionary<string, Type>();

        public static Type FromString(string fullTypeName)
        {
            lock (Cache)
            {
                if (Cache.ContainsKey(fullTypeName))
                {
                    return Cache[fullTypeName];
                }

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    var t = assembly.GetType(fullTypeName, false);

                    if (t == null)
                    {
                        continue;
                    }

                    Cache.Add(fullTypeName, t);
                    return t;
                }

                throw new ArgumentException(
                    "Type " + fullTypeName + " doesn't exist in the current app domain");
            }
        }
    }
}