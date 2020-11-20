using System;
using System.Globalization;
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
                            if (streamEvent.Payload != null)
                            {
                                // TODO: This is a heavy workaround because Json serialization assumes Int64 for numbers
                                var payload = (T) Convert.ChangeType(streamEvent.Payload, typeof(T), CultureInfo.InvariantCulture);
                                observer.OnNext(payload);
                            }
                            break;
                        case StreamEventType.Completion:
                            if (streamEvent.Payload != null)
                            {
                                var payload = (T) Convert.ChangeType(streamEvent.Payload, typeof(T), CultureInfo.InvariantCulture);
                                observer.OnNext(payload);
                            }
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