using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Marble.Generator.Models;
using Marble.Generator.Templates;

namespace Marble.Generator
{
    public class Bootstrap
    {
        public void Run(string projectPath, string targetPath)
        {
            var stopwatch = Stopwatch.StartNew();
            var targetFileInfo = new FileInfo(targetPath);
            var targetNamespace = Path.GetFileNameWithoutExtension(targetFileInfo.Name);
            var targetInformation = new TargetInformation
            {
                Namespace = targetNamespace
            };

            Console.WriteLine("Exploring project " + projectPath);
            var controllerExplorer = new ControllerExplorer(projectPath);
            var controllers = controllerExplorer.Find();

            foreach (var controllerDescriptor in controllers)
            {
                var generatedCode = ClientFileTemplate.Render(targetInformation, controllerDescriptor);
                var targetFilePath =
                    Path.Join(targetFileInfo.Directory.FullName, controllerDescriptor.ClassName + ".cs");
                File.WriteAllText(targetFilePath, generatedCode);
                Console.WriteLine($"Created {controllerDescriptor.Name} client");
            }

            Console.WriteLine($"Generated {controllers.Count()} clients in {stopwatch.ElapsedMilliseconds}ms!");
        }
    }
}