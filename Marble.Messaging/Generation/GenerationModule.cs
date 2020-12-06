using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using DotLiquid;
using Marble.Core.Models;
using Marble.Messaging.Generation.Models;
using Marble.Messaging.Models;
using Marble.Messaging.Utilities;

namespace Marble.Messaging.Generation
{
    public class GenerationModule
    {
        private const string LaunchModeEnvironmentVariableName = "APPLICATION_LAUNCH_MODE";
        private const string OutputDirectoryEnvironmentVariableName = "GENERATION_TARGET_DIR";
        private const string TargetNamespaceEnvironmentVariableName = "GENERATION_TARGET_NAMESPACE";
        private const string TemplatesBasePath = "Marble.Messaging.Generation.Templates.";
        
        private const ApplicationLaunchMode DefaultApplicationLaunchMode = ApplicationLaunchMode.GenerateAndHost;

        private readonly IEnumerable<ControllerDescriptor> descriptors;
        private readonly ApplicationLaunchMode applicationLaunchMode;
        private readonly GenerationSettings generationSettings;
        
        public GenerationModule(IEnumerable<ControllerDescriptor> descriptors)
        {
            this.descriptors = descriptors;
            this.applicationLaunchMode = DefaultApplicationLaunchMode;

            if (Enum.TryParse<ApplicationLaunchMode>(
                Environment.GetEnvironmentVariable(OutputDirectoryEnvironmentVariableName, EnvironmentVariableTarget.Process),
                out var userApplicationLaunchMode))
            {
                this.applicationLaunchMode = userApplicationLaunchMode;
                Console.WriteLine($"Using user provided launch mode {userApplicationLaunchMode}");
            }

            if (this.applicationLaunchMode == ApplicationLaunchMode.Host)
            {
                return;
            }

            this.generationSettings = new GenerationSettings
            {
                OutputDirectory =
                    Environment.GetEnvironmentVariable(OutputDirectoryEnvironmentVariableName,
                        EnvironmentVariableTarget.Process) ?? Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Generated"),
                TargetNamespace = Environment.GetEnvironmentVariable(TargetNamespaceEnvironmentVariableName)
            };
            
            if (!Directory.Exists(this.generationSettings.OutputDirectory))
            {
                Directory.CreateDirectory(this.generationSettings.OutputDirectory);
            }

            this.StartGeneration();

            if (this.applicationLaunchMode == ApplicationLaunchMode.Generate)
            {
                Console.WriteLine("Code generation finished. Exiting application!");
                Environment.Exit(0);
            }
        }

        private void StartGeneration()
        {
            var rawClientTemplate = AssemblyResourceReader.ReadAllText(TemplatesBasePath + "Client.liquid");
            var clientTemplate = Template.Parse(rawClientTemplate);
            var clientGenerator = new ClientGenerator(clientTemplate, this.generationSettings);
            
            foreach (var controllerDescriptor in this.descriptors)
            {
                clientGenerator.Generate(controllerDescriptor);
            }
        }
        
    }
}