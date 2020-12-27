using System.Collections.Generic;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models.Message;
using Marble.Messaging.Transformers;
using Marble.Messaging.Utilities;

namespace Marble.Messaging.Extensions
{
    public static class RequestMessageExtensions
    {
        public static RemoteMessage ToRemoteMessage(this RequestMessage requestMessage,
            ISerializationAdapter serializationAdapter)
        {
            return new RemoteMessage
            {
                Headers = new Dictionary<string, object>
                {
                    {Constants.ControllerHeaderField, requestMessage.Controller},
                    {Constants.ProcedureHeaderField, requestMessage.Procedure},
                    {Constants.CorrelationHeaderField, requestMessage.Correlation},
                    {Constants.ParametersModelType, requestMessage.ParametersModelType}
                },
                Payload = requestMessage.ParametersBytes,
                MessageType = MessageType.RequestMessage,
                Target = ProcedurePath.FromRequestMessage(requestMessage)
            };
        }
    }
}