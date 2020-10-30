using System;
using System.Threading.Tasks;

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
        private IMessagingClient messagingClient;

        public DefaultMathServiceClient(IMessagingClient messagingClient)
        {
            this.messagingClient = messagingClient;
        }

        public Task<int> Add(int a, int b)
        {
            return messagingClient.sendAndReceive("MathGod", "v2", "Add", a, b);
        }

        public Task<int> ComplexAdd(int a, int b)
        {
            return messagingClient.sendAndReceive("MathGod", "v2", "ComplexAdd", a, b);
        }

        public Task<MathResult> Something()
        {
            return messagingClient.sendAndReceive("MathGod", "v2", "Something");
        }
    }

    public class MathResult
    {
        public int SomeInt { get; set; }
    }
}
