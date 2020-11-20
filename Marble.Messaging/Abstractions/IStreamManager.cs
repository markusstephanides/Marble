using System;
using Marble.Messaging.Contracts.Models.Stream;

namespace Marble.Messaging.Abstractions
{
    public interface IStreamManager
    {
        object[] RegisterStreams(object[] input);

        IObservable<T> StreamToObservable<T>(BasicStream stream);
    }
}