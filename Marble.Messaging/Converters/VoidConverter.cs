using System;
using System.Reactive.Linq;
using Marble.Messaging.Abstractions;

namespace Marble.Messaging.Converters
{
    public class VoidConverter : IConverter
    {
        public Type ConversionType { get; set; } = typeof(void);

        public IObservable<object> ConvertToObservable(object obj)
        {
            if (obj.GetType() != this.ConversionType)
            {
                throw new ArgumentException(
                    $"Expected type {this.ConversionType.FullName} got {obj.GetType().FullName} instead!");
            }

            return Observable.Empty<object>();
        }
    }
}