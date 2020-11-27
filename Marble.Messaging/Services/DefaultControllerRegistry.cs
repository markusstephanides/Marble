using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Configuration;
using Marble.Messaging.Converters;
using Marble.Messaging.Explorer;
using Marble.Messaging.Models;
using Marble.Messaging.Transformers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marble.Messaging.Services
{
    public class DefaultControllerRegistry : IControllerRegistry
    {
        private readonly IDictionary<ControllerDescriptor, object> controllers;
        private MessagingConfiguration configuration;

        private ILogger<DefaultControllerRegistry> logger;

        public DefaultControllerRegistry()
        {
            this.AvailableProcedurePaths = new List<string>();
            this.controllers = new ConcurrentDictionary<ControllerDescriptor, object>(
                new ControllerExplorer().ScanAssembly(Assembly.GetEntryAssembly()).Select(
                    controller => new KeyValuePair<ControllerDescriptor, object>(controller, null)
                )
            );

            foreach (var controllerDescriptor in this.controllers.Keys)
            {
                foreach (var procedureDescriptor in controllerDescriptor.ProcedureDescriptors)
                {
                    this.AvailableProcedurePaths.Add(ProcedurePath.FromProcedureDescriptor(procedureDescriptor));
                }
            }
        }

        public List<string> AvailableProcedurePaths { get; set; }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            foreach (var entry in this.controllers)
            {
                serviceCollection.AddSingleton(entry.Key.Type);
            }
        }

        public void OnServiceProviderAvailable(IServiceProvider serviceProvider)
        {
            this.logger = serviceProvider.GetService<ILogger<DefaultControllerRegistry>>();
            this.configuration = serviceProvider.GetService<IOptions<MessagingConfiguration>>().Value;
            foreach (var (descriptor, _) in this.controllers)
            {
                this.controllers[descriptor] = serviceProvider.GetService(descriptor.Type);
                this.logger.LogInformation(
                    $"Found controller instance for {descriptor.Name} containing {descriptor.ProcedureDescriptors.Count()} procedures");
            }
        }

        public IObservable<object> InvokeProcedure(string controllerName, string procedureName, object[]? parameters)
        {
            var (key, value) = this.controllers.First(controllerDescriptor =>
                controllerDescriptor.Key.Name == controllerName);
            var procedureDescriptor =
                key.ProcedureDescriptors.First(procedureDescriptor =>
                    procedureDescriptor.Name == procedureName);
            var procedureMethodInfo = procedureDescriptor.MethodInfo;
            // TODO: do all this logic when the controllers load and not everytime a procedure needs to be invoked

            if (parameters != null)
            {
                var parameterInfos = procedureMethodInfo.GetParameters();
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var parameterInfo = parameterInfos[i];
                    if (parameter.GetType() != parameterInfo.ParameterType)
                    {
                        // TODO: Remove when we have a solution for the int/long deserialization issue
                        parameters[i] = Convert.ChangeType(parameter, parameterInfo.ParameterType);
                    }
                }
            }


            var rawReturnValue = procedureMethodInfo.Invoke(value, parameters);

            var converter = this.configuration.TypeConverters.FirstOrDefault(c =>
                procedureDescriptor.ReturnType == c.ConversionType ||
                procedureDescriptor.ReturnType.IsGenericType &&
                procedureDescriptor.ReturnType.GetGenericTypeDefinition() == c.ConversionType
            );

            converter ??=
                this.configuration.TypeConverters.Find(c => c.GetType() == typeof(ObjectConverter));

            return converter!.ConvertToObservable(rawReturnValue!);

            // if (procedureMethodInfo.ReturnType.IsGenericType &&
            //     procedureMethodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            // {
            //     return ((dynamic) rawReturnValue).Result;
            // }
            //
            // return rawReturnValue;
        }
    }
}