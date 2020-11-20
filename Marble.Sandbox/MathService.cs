using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Declaration;
using Marble.Sandbox.Contracts;
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
        public int Add(int a, int b)
        {
            this.logger.LogInformation($"Addition request incoming with params {a}+{b}!");
            return this.stupidDependency.StupidAdd(a, b);
        }

        [MarbleProcedure]
        public Task<int> ComplexAdd(int a, int b)
        {
            return Task.FromResult(a + b);
        }

        [MarbleProcedure]
        public Task VoidAddTask(int a, int b)
        {
            return Task.FromResult(a + b);
        }

        [MarbleProcedure]
        public void VoidAdd(int a, int b)
        {
            var result = a + b;
        }
        
        [MarbleProcedure]
        public MathResult AddToMathResult(MathResult a, int b)
        {
            return new MathResult
            {
                SomeInt = a.SomeInt + b
            };
        }

        public IObservable<int> StartMathStream(int start)
        {
            return Observable.Timer(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(1000)).Select(
                (t, index) =>
                {
                    Console.WriteLine($"t {t}, index {index}");
                    return start + index;
                });
        }

        // [MarbleProcedure]
        // public MathResult Something(int i)
        // {
        //     return new MathResult
        //     {
        //         SomeInt = i
        //     };
        // }
        //
        // [MarbleProcedure]
        // public ModelMathResult Something2(int i)
        // {
        //     return new ModelMathResult
        //     {
        //         MathResult = i
        //     };
        // }
    }
}