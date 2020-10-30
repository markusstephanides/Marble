using System;
using System.Threading.Tasks;

namespace Marble.Sandbox
{
    // [MarbleController]
    public class MathService
    {
        // [MarbleProcedure]
        public int Add(int a, int b)
        {
            return a + b;
        }

        // [MarbleProcedure]
        public Task<int> ComplexAdd(int a, int b)
        {
            return Task.FromResult(a + b);
        }

        // [MarbleProcedure]
        public MathResult Something()
        {
            return new MathResult();
        }
    }

    public class MathResult
    {
        public int SomeInt { get; set; }
    }
}
