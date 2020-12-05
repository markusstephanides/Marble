using System;
using System.Threading.Tasks;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Models;

namespace Marble.Messaging.Converters
{
    public class GenericTaskResultConverter : IResultConverter
    {
        public Type ConversionInType { get; set; } = typeof(Task<>);
        
        public MessageHandlingResult ConvertResult(object result)
        {
            return new MessageHandlingResult
            {
                Result = ((dynamic)result).Result
            };
        }
    }
}