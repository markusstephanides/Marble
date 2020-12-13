using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marble.Sandbox.Contracts.Models;

namespace Marble.Sandbox.Contracts
{
    public interface IBasketService
    {
        IObservable<List<BasketItem>> GetBasket();
        void UpdateBasket(List<BasketItem> items);
        public Task<bool> Checkout();
    }
}
