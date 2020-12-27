using System;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Models.Message;

namespace Marble.Messaging.Contracts.Abstractions
{
    public interface IMessagingClient
    {
        IObservable<TResult> InvokeProcedureStream<TResult>(string controller, string procedure,
            ParametersModel? messageParameters = null);

        Task<TResult> InvokeProcedureAsync<TResult>(string controller, string procedure,
            ParametersModel? messageParameters = null);

        Task CallProcedureAsync(string controller, string procedure,
            ParametersModel? messageParameters = null);

        Task CallProcedureAsync(RequestMessage requestMessage);
    }
}