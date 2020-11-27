using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Marble.Messaging.Abstractions;

namespace Marble.Messaging.Converters
{
    public class TaskConverter : IConverter
    {
        public Type ConversionType { get; set; } = typeof(Task);

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