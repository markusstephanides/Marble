using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marble.Core.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Messaging.Explorer
{
    public class ClientExplorer : IServicesConfigurable
    {
        private readonly IDictionary<Type, Type> discoveredClients;

        public ClientExplorer()
        {
            this.discoveredClients = new Dictionary<Type, Type>();

            AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach(this.ScanAssembly);
        }

        private void ScanAssembly(Assembly assembly)
        {
            var controllerClientType = typeof(IControllerClient);
            var assemblyTypes = assembly.GetTypes();

            var subjectClientTypes = assemblyTypes
                .Where(type =>
                    type.InheritsOrImplements(controllerClientType) && type != controllerClientType &&
                    type.IsInterface).ToList();

            foreach (var clientInterfaceType in subjectClientTypes)
            {
                var clientImplementationType =
                    assemblyTypes.FirstOrDefault(
                        type => type.InheritsOrImplements(clientInterfaceType) && type.IsClass &&
                                type != clientInterfaceType);

                if (clientImplementationType != null)
                {
                    this.discoveredClients[clientInterfaceType] = clientImplementationType;
                }
            }
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            foreach (var (interfaceType, implementationType) in this.discoveredClients)
            {
                serviceCollection.AddSingleton(interfaceType, implementationType);
            }
        }
    }
}