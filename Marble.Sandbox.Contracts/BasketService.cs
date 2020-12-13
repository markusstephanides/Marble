using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Sandbox.Contracts.Models;

namespace Marble.Sandbox.Contracts
{
    public interface IBasketServiceClient : IControllerClient
    {
        IObservable<List<BasketItem>> GetBasket();

        Task UpdateBasket(List<BasketItem> items);

        Task<bool> Checkout();
    }

    public class DefaultBasketServiceClient : IBasketServiceClient
    {
        private readonly IMessagingClient messagingClient;

        public DefaultBasketServiceClient(IMessagingClient messagingClient)
        {
            this.messagingClient = messagingClient;
        }

        public IObservable<List<BasketItem>> GetBasket()
        {
            return this.messagingClient.InvokeProcedureStream<List<BasketItem>>(
                new RequestMessage("Marble.Sandbox.BasketService", "GetBasket"));
        }

        public Task UpdateBasket(List<BasketItem> items)
        {
            return this.messagingClient.CallProcedureAsync(new RequestMessage("Marble.Sandbox.BasketService",
                "UpdateBasket", items));
        }

        public Task<bool> Checkout()
        {
            return this.messagingClient.InvokeProcedureAsync<bool>(new RequestMessage("Marble.Sandbox.BasketService",
                "Checkout"));
        }
    }
}