using Marble.Messaging.Contracts.Models;

namespace Marble.Messaging.Rabbit.Models
{
    public class RabbitRoutedMessage : RoutedMessage
    {
        public string Exchange { get; set; }
    }
}