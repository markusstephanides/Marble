using Marble.Messaging.Contracts.Models.Stream;

namespace Marble.Messaging.Models
{
    public class NetworkedStreamEvent
    {
        public byte[]? Payload { get; set; }
        public StreamEventType EventType { get; set; }
    }
}