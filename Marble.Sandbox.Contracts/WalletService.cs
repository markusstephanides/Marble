using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Sandbox.Contracts.Models;

namespace Marble.Sandbox.Contracts
{
    public interface IWalletServiceClient : IControllerClient
    {
        IObservable<int> GetUserBalanceStream();

        Task<bool> Checkout(List<BasketItem> items);
    }

    public class DefaultWalletServiceClient : IWalletServiceClient
    {
        private readonly IMessagingClient messagingClient;

        public DefaultWalletServiceClient(IMessagingClient messagingClient)
        {
            this.messagingClient = messagingClient;
        }

        public IObservable<int> GetUserBalanceStream()
        {
            return this.messagingClient.InvokeProcedureStream<int>(new RequestMessage("Marble.Sandbox.WalletService",
                "GetUserBalanceStream"));
        }

        public Task<bool> Checkout(List<BasketItem> items)
        {
            return this.messagingClient.InvokeProcedureAsync<bool>(new RequestMessage("Marble.Sandbox.WalletService",
                "Checkout", items));
        }
    }
}