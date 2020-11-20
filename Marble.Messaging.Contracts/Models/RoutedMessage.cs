using System.Collections.Generic;

namespace Marble.Messaging.Contracts.Models
{
    public class RoutedMessage<TPayload>
    {
        public string CorrelationId { get; set; }
        public string Target { get; set; }
        public TPayload Payload { get; set; }
        public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();
    }
}