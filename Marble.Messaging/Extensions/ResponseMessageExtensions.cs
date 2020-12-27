using System.Collections.Generic;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models.Message;
using Marble.Messaging.Utilities;

namespace Marble.Messaging.Extensions
{
    public static class ResponseMessageExtensions
    {
        public static RemoteMessage ToRemoteMessage(this ResponseMessage responseMessage,
            RequestMessageContext messageContext, ISerializationAdapter serializationAdapter)
        {
            return new RemoteMessage
            {
                Headers = new Dictionary<string, object>
                {
                    {Constants.CorrelationHeaderField, responseMessage.Correlation}
                },
                Payload = serializationAdapter.Serialize(responseMessage.Stream),
                Target = messageContext.ReplyTo,
                MessageType = MessageType.ResponseMessage
            };
        }
    }
}