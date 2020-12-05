using System;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Models;

namespace Marble.Messaging.Converters
{
    public class GenericObservableResultConverter : IResultConverter
    {
        public Type ConversionInType { get; set; } = typeof(IObservable<>);
        public MessageHandlingResult ConvertResult(object result)
        {
            return new MessageHandlingResult
            {
                ResultStream = (IObservable<object>)result
            };
        }
    }
}