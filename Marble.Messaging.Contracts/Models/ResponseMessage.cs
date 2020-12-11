using Marble.Messaging.Contracts.Models.Stream;

namespace Marble.Messaging.Contracts.Models
{
    public class ResponseMessage
    {
        public string Correlation { get; set; }
        public BasicStream Stream { get; set; }
    }
}