using System;

namespace Marble.Messaging.Contracts.Playground.Static
{
    public class ObservableDataFormat<T>
    {
        public T[] Notifications { get; set; }
        public bool Complete { get; set; }

        public bool Error { get; set; }
        // public Exception? Error { get; set; }
    }
}
