using System;
using System.Collections.Generic;
using Marble.Messaging.Contracts.Models.Stream;

namespace Marble.Messaging.Abstractions
{
    public interface IStreamManager
    {
        object[] RegisterStreams(object[] input);

        IObservable<T> TypedStreamToObservable<T>(IEnumerable<TypedStreamEvent<T>> stream);
    }
}