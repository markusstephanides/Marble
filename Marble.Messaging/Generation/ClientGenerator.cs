using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DotLiquid;
using Marble.Messaging.Extensions;
using Marble.Messaging.Generation.Models;
using Marble.Messaging.Models;

namespace Marble.Messaging.Generation
{
    public class ClientGenerator
    {
        private readonly Template template;
        private readonly GenerationSettings generationSettings;

        public ClientGenerator(Template template, GenerationSettings generationSettings)
        {
            this.template = template;
            this.generationSettings = generationSettings;
        }

        public void Generate(ControllerDescriptor descriptor)
        {
            var serviceName = descriptor.Type.Name;
            var outputFileName = $"{serviceName}.cs";

            var usingDirectives = new List<string>()
            {
                "System",
                "System.Threading.Tasks",
                "Marble.Messaging.Contracts.Abstractions",
                "Marble.Messaging.Contracts.Models"
            };

            var procedureModels = new List<Hash>();

            foreach (var procedureDescriptor in descriptor.ProcedureDescriptors)
            {
                var methodInfo = procedureDescriptor.MethodInfo;
                procedureModels.Add(Hash.FromAnonymousObject(new ProcedureTemplateModel
                {
                    MethodName = methodInfo.Name,
                    MethodReturnType = this.GetReturnTypeString(methodInfo)
                }));
            }

            var templateModel = new ClientTemplateModel
            {
                UsingDirectives = usingDirectives,
                Namespace = descriptor.Type.Namespace!,
                ClassName = serviceName,
                ServiceName = descriptor.Name,
                Procedures = procedureModels
            };

            var result = this.template.Render(Hash.FromAnonymousObject(templateModel));
            File.WriteAllText(Path.Join(this.generationSettings.OutputDirectory, outputFileName), result);
        }

        private string GetReturnTypeString(MethodInfo methodInfo)
        {
            if (methodInfo.ReturnType.IsGenericType && (methodInfo.ReturnType.GetGenericTypeDefinition().InheritsOrImplements(typeof(Task<>)) || methodInfo
                .ReturnType.GetGenericTypeDefinition().InheritsOrImplements(typeof(IObservable<>))))
            {
                return methodInfo.ReturnType.GetReadableName();
            }

            if (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(void))
            {
                return nameof(Task);
            }

            return $"{nameof(Task)}<{methodInfo.ReturnType.GetReadableName()}>";
        }
    }
}