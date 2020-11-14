using System.Threading.Tasks;
using Marble.Messaging.Contracts.Models;

namespace Marble.Messaging.Contracts.Abstractions
{
    public interface IMessagingClient
    {
        Task<TResult> SendAsync<TResult>(RequestMessage requestMessage);
        Task SendAndForgetAsync(RequestMessage requestMessage);
    }
}