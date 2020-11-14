namespace Marble.Messaging.Contracts.Models
{
    public class ResponseMessage
    {
        public string CorrelationId { get; set; }
        public object Payload { get; set; }
    }
}