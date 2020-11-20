using System;
using System.Text;
using Marble.Messaging.Contracts.Abstractions;
using Newtonsoft.Json;

namespace Marble.Core.Serialization
{
    public class DefaultJsonSerializationAdapter : ISerializationAdapter
    {
        private readonly Encoding defaultEncoding = Encoding.UTF8;

        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            // TODO Provisional
            TypeNameHandling = TypeNameHandling.All
        };

        public byte[] Serialize(object obj)
        {
            return this.defaultEncoding.GetBytes(JsonConvert.SerializeObject(obj, this.jsonSerializerSettings));
        }

        public object? Deserialize(byte[] bytes, Type type)
        {
            return JsonConvert.DeserializeObject(this.defaultEncoding.GetString(bytes), type,
                this.jsonSerializerSettings);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return (T)Deserialize(bytes, typeof(T));
        }
    }
}