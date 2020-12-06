using System;
using Marble.Messaging.Contracts.Models;

namespace Marble.Messaging.Contracts.Abstractions
{
    public interface IResultConverter
    {
        public Type ConversionInType { get; set; }

        MessageHandlingResult ConvertResult(object result, Type? genericTypeArgument = null!);
    }
}