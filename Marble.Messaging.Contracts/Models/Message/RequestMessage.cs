using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Utilities;

namespace Marble.Messaging.Contracts.Models.Message
{
    public sealed class RequestMessage
    {
        public RequestMessage(string controller, string procedure)
        {
            this.Controller = controller;
            this.Procedure = procedure;
        }

        public RequestMessage()
        {
        }

        public string? Correlation { get; set; }
        public string Controller { get; set; }
        public string Procedure { get; set; }
        public byte[]? ParametersBytes { get; set; }
        public string? ParametersModelType { get; set; }

        public static RequestMessage Create(string controllerName, string procedureName,
            ParametersModel? messageParameters = null, ISerializationAdapter? serializationAdapter = null)
        {
            var requestMessage = new RequestMessage(controllerName, procedureName);

            if (messageParameters is null || serializationAdapter is null)
            {
                return requestMessage;
            }

            requestMessage.ParametersBytes = serializationAdapter.Serialize(messageParameters);
            requestMessage.ParametersModelType = messageParameters.GetType().FullName;

            return requestMessage;
        }

        public ParametersModel? GetParameterModel(ISerializationAdapter serializationAdapter)
        {
            if (this.ParametersBytes is null || this.ParametersModelType is null)
            {
                return null;
            }

            return (ParametersModel) serializationAdapter.Deserialize(this.ParametersBytes,
                TypeLoader.FromString(this.ParametersModelType))!;
        }
    }
}