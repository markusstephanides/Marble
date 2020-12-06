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

                // Add using directive for return type
                if (!methodInfo.ReturnType.IsPrimitive && methodInfo.ReturnType.Namespace != null)
                {
                    if (!usingDirectives.Contains(methodInfo.ReturnType.Namespace))
                    {
                        usingDirectives.Add(methodInfo.ReturnType.Namespace);
                    }
                }
                
                var paramsList = new List<Hash>();
                
                // Add using directives for parameters
                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    var tmpList = new List<string>();
                    var parameterUsingDirectives = parameterInfo.ParameterType.GetUsedNamespaces(ref tmpList);
                    
                    foreach (var parameterUsingDirective in parameterUsingDirectives)
                    {
                        if (parameterUsingDirective != null && !usingDirectives.Contains(parameterUsingDirective))
                        {
                            usingDirectives.Add(parameterUsingDirective);
                        }
                        
                        paramsList.Add(Hash.FromAnonymousObject(new ParameterDescriptor{
                            Name = parameterInfo.Name,
                            ReadableTypeName = parameterInfo.ParameterType.GetReadableName()
                        }));
                    }
                }
                
                procedureModels.Add(Hash.FromAnonymousObject(new ProcedureTemplateModel
                {
                    MethodName = methodInfo.Name,
                    Name = procedureDescriptor.Name,
                    MethodReturnType = this.GetReturnTypeString(methodInfo),
                    ReturnTypeVariant = procedureDescriptor.ReturnTypeVariant.ToString(),
                    PureReturnType = procedureDescriptor.PureReturnType.GetReadableName(),
                    Parameters = paramsList
                }));
            }

            var templateModel = new ClientTemplateModel
            {
                UsingDirectives = usingDirectives,
                Namespace = this.generationSettings.TargetNamespace ?? descriptor.Type.Namespace ?? "Clients",
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