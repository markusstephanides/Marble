using System;

namespace Marble.Messaging.Contracts.Playground
{
    public interface IEncode<I, O>
    {
        IObservable<O> Subscribe(I input);
    }
}
