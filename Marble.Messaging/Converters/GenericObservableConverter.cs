using System;
using Marble.Messaging.Abstractions;

namespace Marble.Messaging.Converters
{
    public class GenericObservableConverter : IConverter
    {
        public Type ConversionType { get; set; } = typeof(IObservable<>);
        
        public IObservable<object> ConvertToObservable(object obj)
        {
            if (obj.GetType() != this.ConversionType)
            {
                throw new ArgumentException(
                    $"Expected type {this.ConversionType.FullName} got {obj.GetType().FullName} instead!");
            }
            
            return (IObservable<object>)obj;
        }
    }
}