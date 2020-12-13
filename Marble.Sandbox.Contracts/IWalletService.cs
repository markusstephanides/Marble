﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marble.Sandbox.Contracts.Models;

namespace Marble.Sandbox.Contracts
{
    public interface IWalletService
    {
        IObservable<int> GetUserBalanceStream();
        public Task<bool> Checkout(List<BasketItem> items);
    }
}
