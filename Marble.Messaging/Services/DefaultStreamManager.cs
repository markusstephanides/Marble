using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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

        public IObservable<T> TypedStreamToObservable<T>(IEnumerable<TypedStreamEvent<T>> events)
        {
            return Observable.Create<T>(observer =>
            {
                foreach (var streamEvent in events)
                {
                    switch (streamEvent.EventType)
                    {
                        case StreamEventType.Notification:
                            if (streamEvent.ContainsPayload)
                            {
                                observer.OnNext(streamEvent.Payload);
                            }

                            break;
                        case StreamEventType.Completion:
                            if (streamEvent.ContainsPayload)
                            {
                                observer.OnNext(streamEvent.Payload);
                            }

                            observer.OnCompleted();
                            break;
                        case StreamEventType.Error:
                            observer.OnError(streamEvent.Exception);
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