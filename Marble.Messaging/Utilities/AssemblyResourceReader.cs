using System.IO;
using System.Reflection;

namespace Marble.Messaging.Utilities
{
    public class AssemblyResourceReader
    {
        public static string ReadAllText(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using Stream stream = assembly.GetManifestResourceStream(path);
            using StreamReader reader = new StreamReader(stream);
            
            return reader.ReadToEnd();
        }
    }
}