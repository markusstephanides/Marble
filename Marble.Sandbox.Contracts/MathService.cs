 using System;
 using System.Threading.Tasks;
 using Marble.Messaging.Contracts.Abstractions;
 using Marble.Messaging.Contracts.Models;
 using Marble.Sandbox.Contracts.Models;


 namespace Marble.Sandbox.Contracts
{
    public interface IMathService : IControllerClient {
         Task<Int32> AddReturnInt();
         Task<Int32> AddReturnTaskInt();
         Task AddReturnTask();
         Task AddReturnVoid();
         Task<MathResult> AddReturnObject();
         Task<MathResult> AddReturnTaskObject();
         IObservable<MathResult> StartMathStreamReturnObject();

    }

    public class DefaultMathServiceClient : IMathService {
       
        private readonly IMessagingClient messagingClient;
        
        public DefaultMathServiceClient(IMessagingClient messagingClient)
        {
            this.messagingClient = messagingClient;
        }

        public Task<int> AddReturnInt()
        {
            throw new NotImplementedException();
        }

        public Task<int> AddReturnTaskInt()
        {
            throw new NotImplementedException();
        }

        public Task AddReturnTask()
        {
            throw new NotImplementedException();
        }

        public Task AddReturnVoid()
        {
            throw new NotImplementedException();
        }

        public Task<MathResult> AddReturnObject()
        {
            throw new NotImplementedException();
        }

        public Task<MathResult> AddReturnTaskObject()
        {
            throw new NotImplementedException();
        }

        public IObservable<MathResult> StartMathStreamReturnObject()
        {
            throw new NotImplementedException();
        }
    }
}