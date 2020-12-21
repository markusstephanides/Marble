using System;

namespace Marble.Messaging.Contracts.Models.Stream
{
    public class TypedStreamEvent<TPayload>
    {
        public StreamEventType EventType { get; set; }
        public TPayload Payload { get; set; }

        public Exception Exception { get; set; }

        public bool ContainsPayload { get; set; }
    }
}