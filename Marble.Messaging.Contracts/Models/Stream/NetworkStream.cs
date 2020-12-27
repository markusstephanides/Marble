using System;
using System.Collections.Generic;
using System.Linq;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Exceptions;
using Marble.Messaging.Contracts.Utilities;

namespace Marble.Messaging.Contracts.Models.Stream
{
    public class NetworkStream
    {
        private static readonly List<Type> unserializableExceptionTypes = new List<Type>();

        public string? ExceptionType { get; set; }
        public List<NetworkedStreamEvent> Events { get; set; }

        public static NetworkStream Completed => new NetworkStream
        {
            Events = new List<NetworkedStreamEvent>
            {
                new NetworkedStreamEvent {EventType = StreamEventType.Completion}
            }
        };

        public static NetworkStream FromResult<T>(T result, ISerializationAdapter serializationAdapter)
        {
            return new NetworkStream
            {
                Events = new List<NetworkedStreamEvent>
                {
                    new NetworkedStreamEvent
                    {
                        EventType = StreamEventType.Completion,
                        Payload = serializationAdapter.Serialize(result)
                    }
                }
            };
        }

        public static NetworkStream FromNotification<T>(T result, ISerializationAdapter serializationAdapter)
        {
            return new NetworkStream
            {
                Events = new List<NetworkedStreamEvent>
                {
                    new NetworkedStreamEvent
                    {
                        EventType = StreamEventType.Notification,
                        Payload = serializationAdapter.Serialize(result)
                    }
                }
            };
        }

        public static NetworkStream FromError(Exception exception, ISerializationAdapter serializationAdapter)
        {
            var exceptionType = exception.GetType();
            var exceptionTypeName = exceptionType.FullName;

            byte[]? serializedException = null;

            void UseExceptionContainer()
            {
                exceptionTypeName = typeof(ExceptionContainer).FullName;
                serializedException = serializationAdapter.Serialize(new ExceptionContainer(exception.Message,
                    exception.GetType().FullName!));
            }

            if (unserializableExceptionTypes.Contains(exceptionType))
            {
                UseExceptionContainer();
            }
            else
            {
                try
                {
                    serializedException = serializationAdapter.Serialize(exception);
                }
                catch (Exception)
                {
                    unserializableExceptionTypes.Add(exceptionType);
                    UseExceptionContainer();
                }
            }

            return new NetworkStream
            {
                ExceptionType = exceptionTypeName,
                Events = new List<NetworkedStreamEvent>
                {
                    new NetworkedStreamEvent
                    {
                        EventType = StreamEventType.Error,
                        Payload = serializedException
                    }
                }
            };
        }

        public static IEnumerable<TypedStreamEvent<TPayload>> ToTypedStream<TPayload>(NetworkStream networkStream,
            ISerializationAdapter serializationAdapter)
        {
            return networkStream.Events.Select(networkedStreamEvent =>
            {
                var typedEvent = new TypedStreamEvent<TPayload>
                {
                    EventType = networkedStreamEvent.EventType
                };

                if (networkedStreamEvent.Payload == null)
                {
                    return typedEvent;
                }

                if (networkedStreamEvent.EventType == StreamEventType.Error)
                {
                    typedEvent.Exception = (Exception) serializationAdapter.Deserialize(networkedStreamEvent.Payload,
                        TypeLoader.FromString(networkStream.ExceptionType!))!;
                }
                else
                {
                    typedEvent.ContainsPayload = true;
                    typedEvent.Payload = serializationAdapter.Deserialize<TPayload>(networkedStreamEvent.Payload);
                }

                return typedEvent;
            });
        }
    }
}