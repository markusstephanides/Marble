namespace Marble.Messaging.Contracts.Models
{
    public sealed class RequestMessage
    {
        public RequestMessage()
        {
        }

        public RequestMessage(string controller, string procedure, params object[]? arguments)
        {
            this.Controller = controller;
            this.Procedure = procedure;
            this.Arguments = arguments;
        }

        public string? Correlation { get; set; }
        public string Controller { get; set; }
        public string Procedure { get; set; }
        public object[]? Arguments { get; set; }
    }
}