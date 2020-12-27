using System;
using System.Threading.Tasks;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models.Message.Handling;

namespace Marble.Messaging.Converters
{
    public class TaskResultConverter : IResultConverter
    {
        public Type ConversionInType { get; set; } = typeof(Task);

        public MessageHandlingResult ConvertResult(object result, Type? genericTypeArgument = null!)
        {
            return new MessageHandlingResult();
        }
    }
}