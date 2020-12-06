using System;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Models;

namespace Marble.Messaging.Converters
{
    public class VoidResultConverter : IResultConverter
    {
        public Type ConversionInType { get; set; } = typeof(void);

        public MessageHandlingResult ConvertResult(object result, Type? genericTypeArgument = null!)
        {
            return new MessageHandlingResult();
        }
    }
}