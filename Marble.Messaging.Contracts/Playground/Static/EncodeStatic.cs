using System;

namespace Marble.Messaging.Contracts.Playground.Static
{
    public class EncodeStatic<T> : IEncode<StaticDataFormat<T>, T>
    {
        public IObservable<T> Subscribe(StaticDataFormat<T> input)
        {
            return System.Reactive.Linq.Observable.Return(input.Value);
        }
    }
}
