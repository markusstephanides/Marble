using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Marble.Messaging.Contracts.Playground.Task;

namespace Marble.Messaging.Contracts.Playground.CoreTask
{
    public class DecodeTask<T> : IDecode<TaskDataFormat<T>, System.Threading.Tasks.Task<T>>
    {
        ISubject<TaskDataFormat<T>> internalSubject = new ReplaySubject<TaskDataFormat<T>>(1);

        public System.Threading.Tasks.Task<T> Instanciate(TaskDataFormat<T> input)
        {
            return System.Reactive.Linq.Observable.Create<T>(observer =>
            {
                var subscription = internalSubject.Subscribe(data =>
                {
                    if (data.IsCancelled)
                    {
                        observer.OnError(null!);
                    }
                    else if (data.HasValue)
                    {
                        observer.OnNext(input.Value);
                        observer.OnCompleted();
                    }
                });

                this.Next(input);

                return () => subscription.Dispose();
            }).ToTask();
        }

        public void Next(TaskDataFormat<T> input)
        {
            this.internalSubject.OnNext(input);
        }

        public System.Threading.Tasks.Task IsFinished()
        {
            return this.internalSubject
                .Where(data => data.HasValue || data.IsCancelled)
                .Take(1)
                .ToTask();
        }
    }
}
