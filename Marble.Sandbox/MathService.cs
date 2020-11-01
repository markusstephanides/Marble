using System;
using System.Threading.Tasks;
using Marble.Core.Declaration;
using Microsoft.Extensions.Logging;

namespace Marble.Sandbox
{
    [MarbleController]
    public class MathService
    {
        private readonly StupidDependency stupidDependency;
        private readonly ILogger<MathService> logger;

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
        public MathResult Something(int i)
        {
            return new MathResult
            {
                SomeInt = i
            };
        }
    }

    public class MathResult
    {
        public int SomeInt { get; set; }
    }
}
