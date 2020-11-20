using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace Marble.Messaging.Contracts.Playground.Static
{
    public class DecodeObservable<O> : IDecode<ObservableDataFormat<O>, IObservable<O>>
    {
        // should be void, not int
        private readonly Subject<int> isFinished = new Subject<int>();

        private readonly ISubject<ObservableDataFormat<O>> internalSubject =
            new ReplaySubject<ObservableDataFormat<O>>(1);

        public IObservable<O> Instanciate(ObservableDataFormat<O> data)
        {
            return System.Reactive.Linq.Observable.Create<O>(observer =>
            {
                var subscription = internalSubject.Subscribe(data =>
                {
                    foreach (var notification in data.Notifications)
                    {
                        observer.OnNext(notification);
                    }

                    if (data.Error)
                    {
                        observer.OnError(null!); // lol
                    }

                    if (data.Complete)
                    {
                        observer.OnCompleted();
                    }
                });

                // Fire initial data
                internalSubject.OnNext(data);

                return () => subscription.Dispose();
            });
        }

        public void Next(ObservableDataFormat<O> input)
        {
            this.internalSubject.Next();
        }

        public System.Threading.Tasks.Task IsFinished()
        {
            return this.internalSubject
                .Where(data => data.Complete || data.Error)
                .Take(1)
                .ToTask();
        }
    }
}
