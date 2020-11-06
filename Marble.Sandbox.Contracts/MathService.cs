using System.Threading.Tasks;
using Marble.Core.Messaging.Abstractions;
using Marble.Core.Messaging.Models;

namespace Marble.Sandbox.Contracts
{
    public interface IMathService
    {
        public Task<int> Add(int a, int b);
        public Task<int> ComplexAdd(int a, int b);
        public Task<MathResult> Something();
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
            return messagingClient.SendAsync<int>(new RequestMessage("Marble.Sandbox.MathService", "Add", a, b));
        }

        public Task<int> ComplexAdd(int a, int b)
        {
            return messagingClient.SendAsync<int>(new RequestMessage("Marble.Sandbox.MathService", "ComplexAdd", a, b));
        }

        public Task<MathResult> Something()
        {
            return messagingClient.SendAsync<MathResult>(new RequestMessage("Marble.Sandbox.MathService", "Something",
                1, 2));
        }
    }

    public class MathResult
    {
        public int SomeInt { get; set; }
    }
}