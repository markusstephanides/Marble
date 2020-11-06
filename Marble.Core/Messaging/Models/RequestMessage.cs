namespace Marble.Core.Messaging.Models
{
    public sealed class RequestMessage
    {
        public RequestMessage(string controller, string procedure, params object[]? arguments)
        {
            Controller = controller;
            Procedure = procedure;
            Arguments = arguments;
        }

        public string Controller { get; }
        public string Procedure { get; }
        public object[]? Arguments { get; }

        public string ResolveQueueName()
        {
            return $"{Controller}:{Procedure}";
        }
    }
}