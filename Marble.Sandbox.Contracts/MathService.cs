using System.Threading.Tasks;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;

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
        private readonly IMessagingAdapter messagingAdapter;

        public DefaultMathServiceClient(IMessagingAdapter messagingAdapter)
        {
            this.messagingAdapter = messagingAdapter;
        }

        public Task<int> Add(int a, int b)
        {
            return this.messagingAdapter.SendAsync<int>(new RequestMessage("Marble.Sandbox.MathService", "Add", a, b));
        }

        public Task<int> ComplexAdd(int a, int b)
        {
            return this.messagingAdapter.SendAsync<int>(new RequestMessage("Marble.Sandbox.MathService", "ComplexAdd", a, b));
        }

        public Task VoidAddTask(int a, int b)
        {
            return this.messagingAdapter.SendAndForgetAsync(new RequestMessage("Marble.Sandbox.MathService", "VoidAddTask", a, b));
        }

        public Task VoidAdd(int a, int b)
        {
            return this.messagingAdapter.SendAndForgetAsync(new RequestMessage("MathService", "VoidAdd", a, b));
        }
    }
}