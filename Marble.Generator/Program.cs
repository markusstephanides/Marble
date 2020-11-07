using System;
using System.IO;

namespace Marble.Generator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Wrong arguments! Format is: [source csproj file] [target csproj file]");
                Environment.Exit(-1);
            }

            var sourceProjectPath = args[0];
            var targetProjectPath = args[1];

            if (!File.Exists(sourceProjectPath) || !sourceProjectPath.EndsWith(".csproj"))
            {
                Console.WriteLine("The source project file you provided is invalid!");
                Environment.Exit(-1);
            }

            if (!File.Exists(targetProjectPath) || !targetProjectPath.EndsWith(".csproj"))
            {
                Console.WriteLine("The target project file you provided is invalid!");
                Environment.Exit(-1);
            }

            new Bootstrap().Run(sourceProjectPath, targetProjectPath);
        }
    }
}