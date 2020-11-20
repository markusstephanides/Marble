using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Models.Stream;

namespace Marble.Messaging.Services
{
    public class DefaultStreamManager : IStreamManager
    {
        public object[] RegisterStreams(object[] input)
        {
            throw new NotImplementedException();
        }

        public IObservable<T> StreamToObservable<T>(BasicStream stream)
        {
            return Observable.Create<T>(observer =>
            {
                foreach (var streamEvent in stream)
                {
                    switch (streamEvent.EventType)
                    {
                        case StreamEventType.Notification:
                            observer.OnNext((T)streamEvent.Payload);
                            break;
                        case StreamEventType.Completion:
                            if (streamEvent.Payload != null) observer.OnNext((T)streamEvent.Payload);
                            observer.OnCompleted();
                            break;
                        case StreamEventType.Error:
                            observer.OnError(new Exception("Sample error"));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                return Disposable.Empty;
            });
        }
    }
}