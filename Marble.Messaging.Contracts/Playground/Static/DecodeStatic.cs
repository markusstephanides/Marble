using System.Threading.Tasks;

namespace Marble.Messaging.Contracts.Playground.Static
{
    public class DecodeStatic<T> : IDecode<T, StaticDataFormat<T>>
    {
        public StaticDataFormat<T> Instanciate(T input)
        {
            return new StaticDataFormat<T>
            {
                Value = input
            };
        }

        public void Next(T input)
        {
            throw new System.Exception("Contract miss-used");
        }

        public System.Threading.Tasks.Task IsFinished()
        {
            return System.Threading.Tasks.Task.FromResult(true);
        }
    }
}
