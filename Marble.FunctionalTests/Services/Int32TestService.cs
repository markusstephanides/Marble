using System;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Declaration;

namespace Marble.FunctionalTests.Services
{
    [MarbleController]
    public class Int32TestService
    {
        public static readonly int ConstantReturnValue = new Random().Next(int.MaxValue);

        [MarbleProcedure]
        public int MethodWithParametersReturnsInt(int value)
        {
            return value;
        }

        [MarbleProcedure]
        public int MethodWithoutParametersReturnsInt()
        {
            return ConstantReturnValue;
        }

        [MarbleProcedure]
        public void MethodWithParametersReturnsVoid(int value)
        {
            // TODO
        }

        [MarbleProcedure]
        public void MethodWithoutParametersReturnsVoid()
        {
            // TODO
        }

        [MarbleProcedure]
        public Task<int> MethodWithoutParametersReturnsTaskInt(int value)
        {
            return Task.FromResult(value);
        }

        [MarbleProcedure]
        public Task<int> MethodWithoutParametersReturnsTaskInt()
        {
            return Task.FromResult(5);
        }

        [MarbleProcedure]
        public Task MethodWithoutParametersReturnsTask(int value)
        {
            // TODO
            return Task.CompletedTask;
        }

        [MarbleProcedure]
        public Task MethodWithoutParametersReturnsTask()
        {
            // TODO
            return Task.CompletedTask;
        }

        [MarbleProcedure]
        public async Task<int> MethodWithoutParametersReturnsAsyncTaskInt(int value)
        {
            return value;
        }

        [MarbleProcedure]
        public async Task<int> MethodWithoutParametersReturnsAsyncTaskInt()
        {
            return 5;
        }

        [MarbleProcedure]
        public async Task MethodWithoutParametersReturnsAsyncTask(int value)
        {
            // TODO
        }

        [MarbleProcedure]
        public async Task MethodWithoutParametersReturnsAsyncTask()
        {
            // TODO
        }
    }
}