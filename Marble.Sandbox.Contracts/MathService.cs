using System.Threading.Tasks;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Models;

namespace Marble.Sandbox.Contracts
{
    public interface IMathService
    {
        Task<int> Add(int a, int b);
        Task<Task<int>> ComplexAdd(int a, int b);
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
            return this.messagingClient.SendAsync<int>(new RequestMessage("Marble.Sandbox.MathService", "Add", a, b));
        }

        public Task<Task<int>> ComplexAdd(int a, int b)
        {
            return this.messagingClient.SendAsync<Task<int>>(new RequestMessage("Marble.Sandbox.MathService",
                "ComplexAdd", a, b));
        }
    }
}