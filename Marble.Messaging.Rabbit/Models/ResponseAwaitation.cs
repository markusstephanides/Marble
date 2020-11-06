using System.Threading.Tasks;

namespace Marble.Messaging.Rabbit.Models
{
    public class ResponseAwaitation
    {
        public TaskCompletionSource<object> TaskCompletionSource { get; set; }
        public long StartTicks { get; set; }
    }
}