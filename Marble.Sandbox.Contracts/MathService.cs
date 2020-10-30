﻿using System;
using System.Threading.Tasks;
using Marble.Core.Messaging.Abstractions;

namespace Marble.Sandbox.Contracts
{
    public interface IMathService
    {
        public Task<int> Add(int a, int b);
        public Task<int> ComplexAdd(int a, int b);
        public Task<MathResult> Something();
    }
    
    public class DefaultMathServiceClient : IMathService
    {
        private IMessagingClient messagingClient;

        public DefaultMathServiceClient(IMessagingClient messagingClient)
        {
            this.messagingClient = messagingClient;
        }

        public Task<int> Add(int a, int b)
        {
            return messagingClient.SendAsync<int>(new RequestMessage("MathGod", "Add", a, b));
        }

        public Task<int> ComplexAdd(int a, int b)
        {
            return messagingClient.SendAsync<int>(new RequestMessage("MathGod", "ComplexAdd", a,b));
        }

        public Task<MathResult> Something()
        {
            return messagingClient.SendAsync<MathResult>(new RequestMessage("MathGod", "Something", 1, 2));
        }
    }

    public class MathResult
    {
        public int SomeInt { get; set; }
    }
}