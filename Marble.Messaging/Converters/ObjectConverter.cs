using System;
using System.Reactive.Linq;
using Marble.Messaging.Abstractions;

namespace Marble.Messaging.Converters
{
    public class ObjectConverter : IConverter
    {
        public Type ConversionType { get; set; } = typeof(object);

        public IObservable<object> ConvertToObservable(object obj)
        {
            return Observable.Return(obj);
        }
    }
}