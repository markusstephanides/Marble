using System;
using System.Threading.Tasks;

namespace Marble.Messaging.Contracts.Playground.Task
{
    public class EncodeTask<T> : IEncode<TaskDataFormat<T>, T>
    {
        public IObservable<T> Subscribe(TaskDataFormat<T> input)
        {
            if (input.IsCancelled)
            {
                return System.Reactive.Linq.Observable.Throw<T>(null!);
            }

            return System.Reactive.Linq.Observable.Return(input.Value);
        }
    }
}
