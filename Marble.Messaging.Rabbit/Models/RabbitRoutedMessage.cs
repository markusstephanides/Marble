using Marble.Core.Messaging.Models;

namespace Marble.Messaging.Rabbit.Models
{
    public class RabbitRoutedMessage : RoutedMessage
    {
        public string Exchange { get; set; }
    }
}