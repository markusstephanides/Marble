using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marble.Sandbox.Contracts.Models;

namespace Marble.Sandbox.Contracts
{
    public interface IBasketService
    {
        IObservable<List<BasketItem>> GetBasket();
        Task UpdateBasket(List<BasketItem> items);
        Task<bool> Checkout();
    }
}
