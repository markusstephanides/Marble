using System;
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

        Task<MathResult> AddToMathResult(MathResult mathResult, int a);

        IObservable<int> StartMathStream(int start);
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
            return this.messagingClient.InvokeProcedureAsync<int>(new RequestMessage("Marble.Sandbox.MathService", "Add", a, b));
        }

        public Task<int> ComplexAdd(int a, int b)
        {
            return this.messagingClient.InvokeProcedureAsync<int>(new RequestMessage("Marble.Sandbox.MathService", "ComplexAdd", a, b));
        }

        public Task VoidAddTask(int a, int b)
        {
            return this.messagingClient.CallProcedureAsync(new RequestMessage("Marble.Sandbox.MathService", "VoidAddTask", a, b));
        }

        public Task VoidAdd(int a, int b)
        {
            return this.messagingClient.CallProcedureAsync(new RequestMessage("Marble.Sandbox.MathService", "VoidAdd", a, b));
        }
        
        public Task<MathResult> AddToMathResult(MathResult mathResult, int a)
        {
            return this.messagingClient.InvokeProcedureAsync<MathResult>(new RequestMessage("Marble.Sandbox.MathService", "AddToMathResult", mathResult, a));
        }
        
        public IObservable<int> StartMathStream(int start)
        {
            return this.messagingClient.InvokeProcedureStream<int>(new RequestMessage("Marble.Sandbox.MathService", "StartMathStream", start));
        }
    }
}