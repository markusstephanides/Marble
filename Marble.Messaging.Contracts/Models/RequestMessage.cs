namespace Marble.Messaging.Contracts.Models
{
    public sealed class RequestMessage
    {
        public string Correlation { get; set; }
        public string Controller { get; set; }
        public string Procedure { get; set; }
        public object[]? Arguments { get; set; }

        public RequestMessage()
        {
            
        }
        
        public RequestMessage(string correlation, string controller, string procedure, params object[]? arguments)
        {
            this.Correlation = correlation;
            this.Controller = controller;
            this.Procedure = procedure;
            this.Arguments = arguments;
        }
        
        public RequestMessage(string controller, string procedure, params object[]? arguments)
        {
            this.Controller = controller;
            this.Procedure = procedure;
            this.Arguments = arguments;
        }
    }
}