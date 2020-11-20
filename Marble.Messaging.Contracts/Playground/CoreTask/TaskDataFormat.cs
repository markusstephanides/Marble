namespace Marble.Messaging.Contracts.Playground.Task
{
    public class TaskDataFormat<T>
    {
        public T Value { get; set; }
        public bool HasValue { get; set; }
        public bool IsCancelled { get; set; }
    }
}
