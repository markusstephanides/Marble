using System;
using Newtonsoft.Json;

namespace Marble.Utilities
{
    public class Serialization
    {
        public static Type GetTypeFromString(string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var t = assembly.GetType(typeName, false);
                if (t != null)
                    return t;
            }

            throw new ArgumentException(
                "Type " + typeName + " doesn't exist in the current app domain");
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}