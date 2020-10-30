namespace Marble.Core.Messaging.Abstractions
{
    public sealed class RequestMessage
    {
        public string CorrelationId { get; set; }
        public string Controller { get; set; }
        public string Procedure { get; set; }
        public object[]? Arguments { get; set; }

        public RequestMessage(string controller, string procedure, params object[]? arguments)
        {
            Controller = controller;
            Procedure = procedure;
            Arguments = arguments;
        }
    }
}