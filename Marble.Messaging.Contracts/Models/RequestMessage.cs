using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Utilities;

namespace Marble.Messaging.Contracts.Models
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
        public byte[]? ArgumentsBytes { get; set; }
        public string? ArgumentsModelType { get; set; }

        public static RequestMessage Create(string controllerName, string procedureName,
            ParametersModel? messageParameters = null, ISerializationAdapter? serializationAdapter = null)
        {
            var requestMessage = new RequestMessage(controllerName, procedureName);

            if (messageParameters is null || serializationAdapter is null)
            {
                return requestMessage;
            }

            requestMessage.ArgumentsBytes = serializationAdapter.Serialize(messageParameters);
            requestMessage.ArgumentsModelType = messageParameters.GetType().FullName;

            return requestMessage;
        }

        public ParametersModel? GetParameterModel(ISerializationAdapter serializationAdapter)
        {
            if (this.ArgumentsBytes is null || this.ArgumentsModelType is null)
            {
                return null;
            }

            return (ParametersModel) serializationAdapter.Deserialize(this.ArgumentsBytes,
                TypeLoader.FromString(this.ArgumentsModelType))!;
        }
    }
}