namespace Marble.Core.Messaging.Models
{
    public class ResponseMessage
    {
        public string CorrelationId { get; set; }
        public object Payload { get; set; }
    }
}