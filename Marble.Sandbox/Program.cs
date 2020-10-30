using Marble.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MarbleCore.Builder
                .ConfigureServices(collection => collection.AddSingleton<StupidDependency>())
                .BuildAndHost();
        }
    }
}
