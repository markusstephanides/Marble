using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Marble.Core.Declaration;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Explorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marble.Core.Messaging
{
    public class MessagingFacade
    {
        private IDictionary<ControllerDescriptor, object> controllers;
        private ILogger<MessagingFacade>? logger;
        private IMessagingClient messagingClient;

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            this.controllers = new ConcurrentDictionary<ControllerDescriptor, object>(
                new ControllerExplorer().ScanAssembly(Assembly.GetEntryAssembly()).Select(
                    controller => new KeyValuePair<ControllerDescriptor, object>(controller, null)
                )
            );

            foreach (var entry in this.controllers)
            {
                serviceCollection.AddSingleton(entry.Key.Type);
            }
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            this.messagingClient = serviceProvider.GetService<IMessagingClient>();
            this.messagingClient.Connect(this, this.controllers.Keys);
            this.logger = serviceProvider.GetService<ILogger<MessagingFacade>>();

            foreach (var (descriptor, _) in this.controllers)
            {
                this.controllers[descriptor] = serviceProvider.GetService(descriptor.Type);
                this.logger?.LogInformation(
                    $"Found controller instance for {descriptor.Name} with {descriptor.ProcedureDescriptors.Count()} procedures");
            }
        }

        public object? InvokeProcedure(string controllerName, string procedureName, object[]? parameters)
        {
            var (key, value) = this.controllers.First(controllerDescriptor =>
                controllerDescriptor.Key.Name == controllerName);
            var procedureMethodInfo =
                key.ProcedureDescriptors.First(procedureDescriptor =>
                    procedureDescriptor.Name == procedureName).MethodInfo;
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
                        parameters[i] = Convert.ChangeType(parameter, parameterInfo.ParameterType);
                    }
                }
            }

            var sw = Stopwatch.StartNew();


            var rawReturnValue = procedureMethodInfo.Invoke(value, parameters);
            Console.WriteLine(sw.ElapsedMilliseconds);

            sw.Restart();

            if (procedureMethodInfo.ReturnType.IsGenericType &&
                procedureMethodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                Console.WriteLine(sw.ElapsedMilliseconds);
                return ((dynamic) rawReturnValue).Result;
            }

            return rawReturnValue;
        }
    }
}