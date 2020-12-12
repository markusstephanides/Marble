using System;
using System.Globalization;
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

        public IObservable<T> StreamToObservable<T>(BasicStream stream)
        {
            var itemType = typeof(T);

            return Observable.Create<T>(observer =>
            {
                foreach (var streamEvent in stream)
                {
                    switch (streamEvent.EventType)
                    {
                        case StreamEventType.Notification:
                            if (streamEvent.Payload != null)
                            {
                                // TODO: This is a heavy workaround because Json serialization assumes Int64 for numbers                        // TODO: Remove when we have a solution for the int/long deserialization issue
                                // TODO: Remove when we have a solution for the int/long deserialization issue
                                var payload = itemType.IsPrimitive
                                    ? (T) Convert.ChangeType(streamEvent.Payload, typeof(T),
                                        CultureInfo.InvariantCulture)
                                    : (T) streamEvent.Payload;
                                observer.OnNext(payload);
                            }

                            break;
                        case StreamEventType.Completion:
                            if (streamEvent.Payload != null)
                            {
                                // TODO: Remove when we have a solution for the int/long deserialization issue
                                var payload = itemType.IsPrimitive
                                    ? (T) Convert.ChangeType(streamEvent.Payload, typeof(T),
                                        CultureInfo.InvariantCulture)
                                    : (T) streamEvent.Payload;
                                observer.OnNext(payload);
                            }

                            observer.OnCompleted();
                            break;
                        case StreamEventType.Error:
                            observer.OnError((Exception) streamEvent.Payload);
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