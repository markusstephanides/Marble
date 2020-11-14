using System.Collections.Generic;

namespace Marble.Messaging.Contracts.Models
{
    public class RoutedMessage
    {
        public string CorrelationId { get; set; }
        public string RoutingKey { get; set; }
        public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();
        public MessageType MessageType { get; set; }
        public object Payload { get; set; }
    }
}