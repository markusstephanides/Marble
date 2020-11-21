using System;

namespace Marble.Messaging.Contracts.Playground.Static
{
    public class EncodeStatic<T> : IEncode<T, StaticDataFormat<T>>
    {
        public IObservable<StaticDataFormat<T>> Subscribe(T input)
        {
            return System.Reactive.Linq.Observable.Return(new StaticDataFormat<T>
            {
                Value = input
            });
        }
    }
}
