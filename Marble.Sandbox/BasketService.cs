using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Declaration;
using Marble.Sandbox.Contracts;
using Marble.Sandbox.Contracts.Models;

namespace Marble.Sandbox
{
    [MarbleController]
    public class BasketService
    {
        private readonly IWalletService walletService;

        private readonly BehaviorSubject<List<BasketItem>>
            basket = new BehaviorSubject<List<BasketItem>>(new List<BasketItem>());

        public BasketService(IWalletService walletService)
        {
            this.walletService = walletService;
        }

        [MarbleProcedure]
        public IObservable<List<BasketItem>> GetBasket()
        {
            return this.basket.AsObservable();
        }

        [MarbleProcedure]
        public Task UpdateBasket(List<BasketItem> items)
        {
            this.basket.OnNext(items);
            return Task.CompletedTask;
        }

        [MarbleProcedure]
        [MarbleChainedProcedure]
        public async Task<bool> Checkout()
        {
            var items = await this.basket.Take(1).ToTask();
            return await this.walletService.Checkout(items);
        }
    }
}
