using System;
using System.Threading.Tasks;
using Marble.Core.Declaration;

namespace Marble.Sandbox
{
    [MarbleController]
    public class MathService
    {
        private readonly StupidDependency stupidDependency;

        public MathService(StupidDependency stupidDependency)
        {
            this.stupidDependency = stupidDependency;
        }

        [MarbleProcedure]
        public int Add(int a, int b)
        {
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
