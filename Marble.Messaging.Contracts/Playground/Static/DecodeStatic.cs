using System.Threading.Tasks;

namespace Marble.Messaging.Contracts.Playground.Static
{
    public class DecodeStatic<T> : IDecode<StaticDataFormat<T>, T>
    {
        public T Instanciate(StaticDataFormat<T> input)
        {
            return input.Value;
        }

        public void Next(StaticDataFormat<T> input)
        {
            throw new System.Exception("Contract miss-used");
        }

        public System.Threading.Tasks.Task IsFinished()
        {
            return System.Threading.Tasks.Task.FromResult(true);
        }
    }
}
