using System;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Sandbox.Contracts.Models;

namespace Marble.Sandbox.Contracts
{
    public interface IMathService : IControllerClient {
        
        Task<int> AddReturnInt(int a, int b);
        
        Task<int> AddReturnTaskInt(int a, int b);
        
        Task AddReturnTask(int a, int b);
        
        Task AddReturnVoid(int a, int b);
        
        Task<MathResult> AddReturnObject(int a, int b);
        
        Task<MathResult> AddReturnTaskObject(int a, int b);
        
        IObservable<int> StartMathStreamReturnInt(int start);
        
        IObservable<MathResult> StartMathStreamReturnObject(int start);

    }

    public class DefaultMathServiceClient : IMathService {
       
        private readonly IMessagingClient messagingClient;
        
        public DefaultMathServiceClient(IMessagingClient messagingClient)
        {
            this.messagingClient = messagingClient;
        }

        public Task<int> AddReturnInt(int a, int b){
            return this.messagingClient.InvokeProcedureAsync<int>(new RequestMessage("Marble.Sandbox.MathService", "AddaReturnInt", a, b));
        }
    
        public Task<int> AddReturnTaskInt(int a, int b){
            return this.messagingClient.InvokeProcedureAsync<int>(new RequestMessage("Marble.Sandbox.MathService", "AddReturnTaskInt", a, b));
        }
    
        public Task AddReturnTask(int a, int b){
            return this.messagingClient.CallProcedureAsync(new RequestMessage("Marble.Sandbox.MathService", "AddReturnTask", a, b));
        }
    
        public Task AddReturnVoid(int a, int b){
            return this.messagingClient.CallProcedureAsync(new RequestMessage("Marble.Sandbox.MathService", "AddReturnVoid", a, b));
        }
    
        public Task<MathResult> AddReturnObject(int a, int b){
            return this.messagingClient.InvokeProcedureAsync<MathResult>(new RequestMessage("Marble.Sandbox.MathService", "AddReturnObject", a, b));
        }
    
        public Task<MathResult> AddReturnTaskObject(int a, int b){
            return this.messagingClient.InvokeProcedureAsync<MathResult>(new RequestMessage("Marble.Sandbox.MathService", "AddReturnTaskObject", a, b));
        }
    
        public IObservable<int> StartMathStreamReturnInt(int start){
            return this.messagingClient.InvokeProcedureStream<int>(new RequestMessage("Marble.Sandbox.MathService", "StartMathStreamasdasdasdReturnInt", start));
        }
    
        public IObservable<MathResult> StartMathStreamReturnObject(int start){
            return this.messagingClient.InvokeProcedureStream<MathResult>(new RequestMessage("Marble.Sandbox.MathService", "StartMathStreamReturnObject", start));
        }
    

    }
}