using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Declaration;
using Marble.Sandbox.Contracts.Models;

namespace Marble.Sandbox
{
    [MarbleController]
    public class WalletService
    {
        private readonly BehaviorSubject<int>
            userBalanceInCent = new BehaviorSubject<int>(1000);

        [MarbleProcedure]
        public IObservable<int> GetUserBalanceStream()
        {
            return this.userBalanceInCent.AsObservable();
        }

        /// Method that will be used as a chain
        [MarbleProcedure]
        public async Task<bool> Checkout(List<BasketItem> items)
        {
            var currentBalance = await this.userBalanceInCent.Take(1).ToTask();
            var basketItemsCostInCent = items.Select(i => i.PriceInCent).Count();
            var canBuy = currentBalance >= basketItemsCostInCent;

            if (canBuy)
            {
                this.userBalanceInCent.OnNext(currentBalance - basketItemsCostInCent);
            }

            return canBuy;
        }
    }
}