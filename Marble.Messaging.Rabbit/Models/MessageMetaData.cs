namespace Marble.Messaging.Rabbit.Models
{
    public class MessageMetaData
    {
        public string CorrelationId { get; set; }
        public string ReplyToQueue { get; set; }
    }
}