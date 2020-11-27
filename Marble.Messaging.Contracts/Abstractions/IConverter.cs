using System;

namespace Marble.Messaging.Abstractions
{
    public interface IConverter
    {
        public Type ConversionType { get; set; }

        IObservable<object> ConvertToObservable(object obj);
    }
}