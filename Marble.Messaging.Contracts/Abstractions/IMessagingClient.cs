using System;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Models;

namespace Marble.Messaging.Contracts.Abstractions
{
    public interface IMessagingClient
    {
        IObservable<TResult> InvokeProcedureStream<TResult>(string controller, string procedure, params object[] parameters);
        IObservable<TResult> InvokeProcedureStream<TResult>(RequestMessage requestMessage);
        Task<TResult> InvokeProcedureAsync<TResult>(string controller, string procedure, params object[] parameters);
        Task<TResult> InvokeProcedureAsync<TResult>(RequestMessage requestMessage);
        Task CallProcedureAsync(string controller, string procedure, params object[] parameters);
        Task CallProcedureAsync(RequestMessage requestMessage);
    }
}