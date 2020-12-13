using System;
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

            this.logger.LogInformation("MathService created!");
        }

        [MarbleProcedure]
        public int AddReturnInt(int a, int b)
        {
            this.logger.LogInformation($"AddReturnInt request incoming with params {a}+{b}!");
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
            this.logger.LogInformation($"AddReturnTaskInt request incoming with params {a}+{b}!");
            return Task.FromResult(a + b);
        }

        [MarbleProcedure]
        public Task AddReturnTask(int a, int b)
        {
            this.logger.LogInformation($"AddReturnTask request incoming with params {a}+{b}!");
            return Task.FromResult(a + b);
        }

        [MarbleProcedure]
        public void AddReturnVoid(int a, int b)
        {
            this.logger.LogInformation($"AddReturnVoid request incoming with params {a}+{b}!");
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
        public Task<MathResult> AddReturnTaskObjectThrowException(int a, int b)
        {
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