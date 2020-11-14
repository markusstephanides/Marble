using Marble.Core;
using Marble.Logging;
using Marble.Messaging.Rabbit.Extensions;
using Marble.Utilities.Extensions;

namespace Marble.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MarbleCore.Builder
                .AddSingleton<StupidDependency>()
                .WithRabbitMessaging()
                .WithLogging()
                .BuildAndHost();
        }
    }
}