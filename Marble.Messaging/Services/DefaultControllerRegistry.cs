using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Configuration;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Converters;
using Marble.Messaging.Exceptions;
using Marble.Messaging.Explorer;
using Marble.Messaging.Extensions;
using Marble.Messaging.Generation;
using Marble.Messaging.Models;
using Marble.Messaging.Transformers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Marble.Messaging.Services
{
    public class DefaultControllerRegistry<TConfiguration> : IControllerRegistry
        where TConfiguration : MessagingConfiguration
    {
        private readonly IDictionary<ControllerDescriptor, object> controllers;
        private TConfiguration configuration;

        private ILogger<DefaultControllerRegistry<TConfiguration>> logger;

        public DefaultControllerRegistry()
        {
            this.AvailableProcedurePaths = new List<string>();

            var definitions = this.LoadControllerDefinitions();

            this.controllers = new ConcurrentDictionary<ControllerDescriptor, object>(definitions
                .Select(definition => new KeyValuePair<ControllerDescriptor, object>(definition, null!)));

            var generationModule = new GenerationModule(definitions);

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
            this.logger = serviceProvider.GetService<ILogger<DefaultControllerRegistry<TConfiguration>>>();
            this.configuration = serviceProvider.GetService<IOptions<TConfiguration>>().Value;
            foreach (var (descriptor, _) in this.controllers)
            {
                this.controllers[descriptor] = serviceProvider.GetService(descriptor.Type);
                this.logger.LogInformation(
                    "Found controller instance for {DescriptorName} containing {ProcedureCount} procedures",
                    descriptor.Name, descriptor.ProcedureDescriptors.Count());
            }
        }

        public MessageHandlingResult InvokeProcedure(string controllerName, string procedureName, object[]? parameters)
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

            object? rawReturnValue = null;

            try
            {
                rawReturnValue = procedureMethodInfo.Invoke(value, parameters);
            }
            catch (TargetInvocationException e)
            {
                throw new ProcedureInvocationException(
                    "Target invocation exception thrown during execution of procedure.", e.InnerException);
            }
            catch (Exception e)
            {
                throw new ProcedureInvocationException("Exception thrown during execution of procedure.", e);
            }

            try
            {
                var converter = this.configuration.TypeConverters.FirstOrDefault(c =>
                    procedureDescriptor.ReturnType.InheritsOrImplements(c.ConversionInType)
                ) ?? this.configuration.TypeConverters.Find(c => c.GetType() == typeof(ObjectResultConverter));

                return converter!.ConvertResult(rawReturnValue,
                    procedureDescriptor.ReturnType.IsGenericType
                        ? procedureDescriptor.ReturnType.GenericTypeArguments[0]
                        : null);
            }
            catch (Exception e)
            {
                throw new ProcedureResultConversionException("Exception thrown during conversion of procedure result",
                    e);
            }
        }

        private IList<ControllerDescriptor> LoadControllerDefinitions()
        {
            var targetAssembly = Assembly.GetEntryAssembly();
            var allControllers = new ControllerExplorer().ScanAssembly(targetAssembly).ToList();
            var containsErrors = false;

            foreach (var controllerDescriptor in allControllers)
            {
                var controllerNameCount = allControllers.Count(d => d.Name == controllerDescriptor.Name);

                if (controllerNameCount > 1)
                {
                    Console.WriteLine(
                        $"Error while loading controller definitions! The controller name {controllerDescriptor.Name} is not unique and currently exists {controllerNameCount} times!");
                    containsErrors = true;
                }

                foreach (var procedureDescriptor in controllerDescriptor.ProcedureDescriptors)
                {
                    var procedureNameCount = allControllers.Count(d => d.Name == procedureDescriptor.Name);

                    if (procedureNameCount > 1)
                    {
                        Console.WriteLine(
                            $"Error while loading procedure definitions of {controllerDescriptor.Name}! The procedure name {procedureDescriptor.Name} is not unique and currently exists {procedureNameCount} times!");
                        containsErrors = true;
                    }
                }
            }

            if (containsErrors)
            {
                Console.WriteLine("Errors found while exploring controllers! Aborting startup.");
                Environment.Exit(-1);
            }

            return allControllers;
        }
    }
}