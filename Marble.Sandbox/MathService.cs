using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Declaration;
using Marble.Sandbox.Contracts.Models;
using Microsoft.Extensions.Logging;

namespace Marble.Sandbox
{
    [MarbleController]
    public class MathService
    {
        private readonly ILogger<MathService> logger;
        private readonly StupidDependency stupidDependency;

        public MathService(StupidDependency stupidDependency, ILogger<MathService> logger)
        {
            this.stupidDependency = stupidDependency;
            this.logger = logger;
        }

        [MarbleProcedure]
        public int AddReturnInt(int a, int b)
        {
            return this.stupidDependency.StupidAdd(a, b);
        }

        [MarbleProcedure]
        public int AddReturnIntThrowException(int a, int b)
        {
            throw new ArgumentException("Some exception message", nameof(a));
        }

        [MarbleProcedure]
        public Task<int> AddReturnTaskInt(int a, int b)
        {
            return Task.FromResult(a + b);
        }

        [MarbleProcedure]
        public Task AddReturnTask(int a, int b)
        {
            this.logger.LogInformation("AddReturnTask request incoming with params {a}+{b}!", a, b);
            return Task.FromResult(a + b);
        }

        [MarbleProcedure]
        public void AddReturnVoid(int a, int b)
        {
            this.logger.LogInformation($"AddReturnVoid request incoming with params {a}+{b}!", a, b);
            var result = a + b;
        }

        [MarbleProcedure]
        public MathResult AddReturnObject(int a, int b)
        {
            return new MathResult
            {
                SomeInt = a + b
            };
        }

        [MarbleProcedure]
        public Task<MathResult> AddReturnTaskObject(int a, int b)
        {
            return Task.FromResult(new MathResult
            {
                SomeInt = a + b
            });
        }

        [MarbleProcedure]
        public Task<List<MathResult>> ReturnListTask(int a, int b)
        {
            return Task.FromResult(new List<MathResult> {new MathResult {SomeInt = a}, new MathResult {SomeInt = b}});
        }

        [MarbleProcedure]
        public List<MathResult> ReturnList(int a, int b)
        {
            return new List<MathResult> {new MathResult {SomeInt = a}, new MathResult {SomeInt = b}};
        }


        [MarbleProcedure]
        public Task<List<int>> ReturnListIntTask(int a, int b)
        {
            return Task.FromResult(new List<int> {a, b});
        }

        [MarbleProcedure]
        public List<int> ReturnListInt(int a, int b)
        {
            return new List<int> {a, b};
        }

        [MarbleProcedure]
        public Guid ReturnGuid(int a, int b)
        {
            return Guid.NewGuid();
        }

        [MarbleProcedure]
        public Guid? ReturnNullGuid(int a, int b)
        {
            return null;
        }

        [MarbleProcedure]
        public MathResult ReturnNull(int a, int b)
        {
            return null;
        }

        [MarbleProcedure]
        public async Task<MathResult> AddReturnTaskObjectThrowException(int a, int b)
        {
            await Task.Delay(30);
            throw new ApplicationException("Application exception");
        }

        [MarbleProcedure]
        public IObservable<int> StartMathStreamReturnInt(int start)
        {
            return Observable.Timer(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(1000)).Select(
                (t, index) => start + index).Take(5);
        }

        [MarbleProcedure]
        public IObservable<MathResult> StartMathStreamReturnObject(int start)
        {
            return Observable.Timer(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(1000)).Select(
                (t, index) => new MathResult {SomeInt = start + index}).Take(5);
        }

        [MarbleProcedure]
        public IObservable<MathResult> StartMathStreamReturnObjectThrowException(int start)
        {
            return Observable.Timer(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(1000)).Select(
                (t, index) =>
                {
                    if (t == 2)
                    {
                        throw new ArithmeticException("Some message");
                    }

                    return new MathResult {SomeInt = start + index};
                }).Take(5);
        }
    }
}