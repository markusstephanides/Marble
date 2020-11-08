using System.Threading.Tasks;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Models;

namespace Marble.Sandbox.Contracts
{
    public interface IMathService
    {
        Task<int> Add(int a, int b);
        Task<int> ComplexAdd(int a, int b);
        Task VoidAddTask(int a, int b);
        Task VoidAdd(int a, int b);
    }

    public class DefaultMathServiceClient : IMathService
    {
        private readonly IMessagingClient messagingClient;

        public DefaultMathServiceClient(IMessagingClient messagingClient)
        {
            this.messagingClient = messagingClient;
        }

        public Task<int> Add(int a, int b)
        {
            return this.messagingClient.SendAsync<int>(new RequestMessage("MathService", "Add", a, b));
        }

        public Task<int> ComplexAdd(int a, int b)
        {
            return this.messagingClient.SendAsync<int>(new RequestMessage("MathService", "ComplexAdd", a, b));
        }

        public Task VoidAddTask(int a, int b)
        {
            return this.messagingClient.SendAndForgetAsync(new RequestMessage("MathService", "VoidAddTask", a, b));
        }

        public Task VoidAdd(int a, int b)
        {
            return this.messagingClient.SendAndForgetAsync(new RequestMessage("MathService", "VoidAdd", a, b));
        }
    }
}