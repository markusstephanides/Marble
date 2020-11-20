namespace Marble.Messaging.Contracts.Models.Stream
{
    public class StreamEvent
    {
        public object Payload { get; set; }
        public StreamEventType EventType { get; set; }
    }
}