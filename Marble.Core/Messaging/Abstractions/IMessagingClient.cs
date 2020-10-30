using System.Threading.Tasks;

namespace Marble.Core.Messaging.Abstractions
{
    public interface IMessagingClient
    {
        Task<TResult> SendAsync<TResult>(RequestMessage requestMessage);
        Task SendAndForgetAsync(RequestMessage requestMessage);
        Task Reply<TResult>(string correlationId, TResult result);
    }
}