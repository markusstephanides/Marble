using System.Threading.Tasks;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Models;

namespace Marble.Sandbox.Contracts
{
    public interface IMathService
    {
        Task<int> Add(int a, int b);
        Task<Task<int>> ComplexAdd(int a, int b);
        Task<MathResult> Something(int i);
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
            return messagingClient.SendAsync<int>(new RequestMessage("MathService", "Add", a, b));
        }

        public Task<Task<int>> ComplexAdd(int a, int b)
        {
            return messagingClient.SendAsync<Task<int>>(new RequestMessage("MathService", "ComplexAdd", a, b));
        }

        public Task<MathResult> Something(int i)
        {
            return messagingClient.SendAsync<MathResult>(new RequestMessage("MathService", "Something", i));
        }
    }
}