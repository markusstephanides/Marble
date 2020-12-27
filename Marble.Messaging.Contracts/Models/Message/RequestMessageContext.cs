namespace Marble.Messaging.Contracts.Models.Message
{
    public class RequestMessageContext
    {
        public string ReplyTo { get; set; }
        public RequestMessage RequestMessage { get; set; }
    }
}