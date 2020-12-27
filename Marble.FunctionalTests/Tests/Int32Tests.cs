using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Marble.Core;
using Marble.FunctionalTests.Clients;
using Marble.FunctionalTests.Services;
using Marble.Messaging.Rabbit.Configuration;
using Marble.Messaging.Rabbit.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Marble.FunctionalTests.Tests
{
    public class Int32Tests
    {
        private IInt32TestServiceClient client;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // TODO: Maybe it would be better to use one marble instance atomically, so one for each test and not one for all tests
            var additionalAssemblies = new List<Assembly> {Assembly.GetAssembly(typeof(Int32Tests))};
            var host = MarbleCore.Builder
                .WithRabbitMessaging(defaultConfiguration: new RabbitConfiguration
                {
                    ConnectionString = "amqp://guest:guest@localhost:5672"
                }, additionalAssemblies: additionalAssemblies)
                .BuildAndHost();

            this.client = host.ServiceProvider.GetService<IInt32TestServiceClient>();
        }

        [Test]
        public async Task MethodWithParametersReturnsInt_ShouldReturnTheGivenInteger([Random(1)] int expected)
        {
            var returnedValue = await this.client.MethodWithParametersReturnsInt(expected);
            returnedValue.Should().Be(expected);
        }

        [Test]
        public async Task MethodWithoutParametersReturnsInt_ShouldReturnTheConstantInteger()
        {
            var expected = Int32TestService.ConstantReturnValue;
            var returnedValue = await this.client.MethodWithParametersReturnsInt(expected);
            returnedValue.Should().Be(expected);
        }
    }
}