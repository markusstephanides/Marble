using System;
using System.Reactive.Linq;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Models;

namespace Marble.Messaging.Converters
{
    public class ObjectResultConverter : IResultConverter
    {
        public Type ConversionInType { get; set; } = typeof(object);
        
        public MessageHandlingResult ConvertResult(object result, Type? genericTypeArgument = null!)
        {
            return new MessageHandlingResult
            {
                Result = result
            };
        }
    }
}