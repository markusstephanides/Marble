using Marble.Messaging.Contracts.Models.Stream;

namespace Marble.Messaging.Contracts.Models.Message
{
    public class ResponseMessage
    {
        public string Correlation { get; set; }
        public NetworkStream Stream { get; set; }
    }
}