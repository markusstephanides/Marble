using System;

namespace Marble.Messaging.Contracts.Models
{
    public class MessageHandlingResult
    {
        public IObservable<object>? ResultStream { get; set; }
        public object? Result { get; set; }
        
        public MessageHandlingResultType Type
        {
            get
            {
                if (this.ResultStream is null && this.Result != null)
                {
                    return MessageHandlingResultType.Single;
                }

                if (this.ResultStream != null && this.Result is null)
                {
                    return MessageHandlingResultType.Stream;
                }
                
                if (this.ResultStream != null && this.Result != null)
                {
                    throw new Exception("Only ResultStream OR Result can be set.");
                }

                return MessageHandlingResultType.Void;
            }
        }
    }
}