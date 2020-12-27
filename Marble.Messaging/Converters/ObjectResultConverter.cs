using System;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models.Message.Handling;

namespace Marble.Messaging.Converters
{
    public class ObjectResultConverter : IResultConverter
    {
        public Type ConversionInType { get; set; } = typeof(object);

        public MessageHandlingResult ConvertResult(object result, Type? genericTypeArgument = null!)
        {
            return new MessageHandlingResult(result);
        }
    }
}