using System.Collections.Generic;

namespace Marble.Messaging.Contracts.Models
{
    public class RemoteMessage
    {
        public string Target { get; set; }
        public string ReplyTo { get; set; }
        public Dictionary<string, object> Headers { get; set; }
        public MessageType MessageType { get; set; }
        public byte[] Payload { get; set; }
    }
}