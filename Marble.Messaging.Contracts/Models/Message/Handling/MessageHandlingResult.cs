using System;

namespace Marble.Messaging.Contracts.Models.Message.Handling
{
    public class MessageHandlingResult
    {
        public MessageHandlingResult()
        {
            this.Type = MessageHandlingResultType.Void;
        }

        public MessageHandlingResult(IObservable<object>? resultStream)
        {
            this.Type = MessageHandlingResultType.Stream;
            this.ResultStream = resultStream;
        }

        public MessageHandlingResult(object? result)
        {
            if (result is null)
            {
                this.Type = MessageHandlingResultType.Empty;
            }
            else
            {
                this.Type = MessageHandlingResultType.Single;
                this.Result = result;
            }
        }

        public IObservable<object>? ResultStream { get; set; }
        public object? Result { get; set; }
        public MessageHandlingResultType Type { get; set; }
    }
}