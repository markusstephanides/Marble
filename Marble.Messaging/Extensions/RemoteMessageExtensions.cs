﻿using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Contracts.Models.Stream;
using Marble.Messaging.Utilities;

namespace Marble.Messaging.Extensions
{
    public static class RemoteMessageExtensions
    {
        public static ResponseMessage ToResponseMessage(this RemoteMessage remoteMessage,
            ISerializationAdapter serializationAdapter)
        {
            return new ResponseMessage
            {
                Stream = serializationAdapter.Deserialize<NetworkStream>(remoteMessage.Payload),
                Correlation = remoteMessage.Headers[Constants.CorrelationHeaderField] as string
            };
        }

        public static RequestMessageContext ToRequestMessageContext(this RemoteMessage remoteMessage,
            ISerializationAdapter serializationAdapter)
        {
            return new RequestMessageContext
            {
                RequestMessage = new RequestMessage
                {
                    Controller = remoteMessage.Headers[Constants.ControllerHeaderField] as string,
                    Procedure = remoteMessage.Headers[Constants.ProcedureHeaderField] as string,
                    Correlation = remoteMessage.Headers[Constants.CorrelationHeaderField] as string,
                    ArgumentsModelType = remoteMessage.Headers[Constants.ArgumentsModelType] as string,
                    ArgumentsBytes = remoteMessage.Payload
                },
                ReplyTo = remoteMessage.ReplyTo
            };
        }
    }
}