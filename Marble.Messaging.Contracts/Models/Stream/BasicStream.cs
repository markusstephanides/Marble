using System.Collections.Generic;

namespace Marble.Messaging.Contracts.Models.Stream
{
    public class BasicStream: List<StreamEvent>
    {
        public static BasicStream FromResult<T>(T result)
        {
            return new BasicStream
            {
                new StreamEvent
                {
                    EventType = StreamEventType.Completion,
                    Payload = result
                }
            };
        }
        
        public static BasicStream FromError<T>(T result)
        {
            return new BasicStream
            {
                new StreamEvent
                {
                    EventType = StreamEventType.Error,
                    Payload = result
                }
            };
        }
    }
}