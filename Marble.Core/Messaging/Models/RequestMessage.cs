namespace Marble.Core.Messaging.Models
{
    public sealed class RequestMessage
    {
        public RequestMessage(string controller, string procedure, params object[]? arguments)
        {
            this.Controller = controller;
            this.Procedure = procedure;
            this.Arguments = arguments;
        }

        public string Controller { get; }
        public string Procedure { get; }
        public object[]? Arguments { get; }
    }
}