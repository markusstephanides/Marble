using System;
using System.Threading.Tasks;
using Marble.Core;
using Marble.Messaging.Rabbit.Extensions;
using Marble.Utilities.Extensions;

namespace Marble.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var methodInfo = typeof(MathService).GetMethod("ComplexAdd");

            Console.WriteLine(methodInfo.ReturnType == typeof(Task));
            Console.WriteLine(methodInfo.ReturnType == typeof(Task<>));
            Console.WriteLine(methodInfo.ReturnType == typeof(Task<int>));
            Console.WriteLine(methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
            Console.WriteLine(methodInfo.ReturnType.FullName);
            Console.WriteLine(methodInfo.ReturnType.GetGenericTypeDefinition().FullName);
            
            MarbleCore.Builder
                .AddSingleton<StupidDependency>()
                .WithRabbitMessaging()
                .BuildAndHost();
        }
    }
}