namespace Marble.Messaging.Contracts.Models.Stream
{
    public class NetworkedStreamEvent
    {
        public byte[]? Payload { get; set; }
        public StreamEventType EventType { get; set; }
    }
}