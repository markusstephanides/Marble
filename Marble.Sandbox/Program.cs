using Marble.Core;
using Marble.Messaging.Rabbit.Extensions;
using Marble.Utilities.Extensions;

namespace Marble.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // var obs = Observable.Create<int>(observer =>
            // {
            //     observer.OnError(new ArgumentException());
            //     return () => { };
            // });
            //
            // obs.Subscribe();

            MarbleCore.Builder
                .AddSingleton<StupidDependency>()
                .WithRabbitMessaging()
                .BuildAndHost();
        }
    }
}