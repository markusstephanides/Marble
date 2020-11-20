using System;
using Marble.Messaging.Contracts.Playground.Static;

namespace Marble.Messaging.Contracts.Playground.Observable
{
    public class EncodeObservable<I>: IEncode<IObservable<I>, ObservableDataFormat<I>>
    {
        public IObservable<ObservableDataFormat<I>> Subscribe(IObservable<I> input)
        {
            return System.Reactive.Linq.Observable.Create<ObservableDataFormat<I>>(observer =>
            {
                var hasSendFirstEvent = false;

                var subscription = input.Subscribe(data =>
                {
                    observer.OnNext(new ObservableDataFormat<I>()
                    {
                        Notifications = new[] {data},
                        Complete = false,
                        Error = false
                    });
                    hasSendFirstEvent = true;
                }, error =>
                {
                    observer.OnNext(new ObservableDataFormat<I>()
                    {
                        Notifications = new I[] { },
                        Complete = false,
                        Error = true
                    });
                    observer.OnError(error);
                    hasSendFirstEvent = true;
                }, () =>
                {
                    observer.OnNext(new ObservableDataFormat<I>()
                    {
                        Notifications = new I[] { },
                        Complete = true,
                        Error = false
                    });
                    observer.OnCompleted();
                    hasSendFirstEvent = true;
                });

                if (!hasSendFirstEvent)
                {
                    // Send a first event with no notifications so this observable tells it's finished
                    observer.OnNext(new ObservableDataFormat<I>()
                    {
                        Notifications = new I[] { },
                        Complete = true,
                        Error = false
                    });
                }

                return () => subscription.Dispose();
            });
        }
    }
}
