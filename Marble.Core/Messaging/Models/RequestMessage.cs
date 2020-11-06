namespace Marble.Core.Messaging.Abstractions
{
    public sealed class RequestMessage
    {
        public string Controller { get; }
        public string Procedure { get; }
        public object[]? Arguments { get; }

        public RequestMessage(string controller, string procedure, params object[]? arguments)
        {
            Controller = controller;
            Procedure = procedure;
            Arguments = arguments;
        }

        public string ResolveQueueName()
        {
            return $"{this.Controller}:{this.Procedure}";
        }
    }
}