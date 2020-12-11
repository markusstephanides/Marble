using System;
using System.Text;
using Marble.Messaging.Contracts.Abstractions;
using Newtonsoft.Json;

namespace Marble.Core.Serialization.Json
{
    public class DefaultJsonSerializationAdapter : ISerializationAdapter
    {
        private readonly Encoding defaultEncoding = Encoding.UTF8;

        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
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
            return (T) this.Deserialize(bytes, typeof(T));
        }

        private object PackIntoContainer(object input)
        {
            return input switch
            {
                int _ => new Int32Container(input),
                short _ => new Int16Container(input),
                byte _ => new Int8Container(input),
                decimal _ => new DecimalContainer(input),
                float _ => new FloatContainer(input),
                double _ => new DoubleContainer(input),
                _ => input
            };
        }
        
        private object UnpackFromContainer(object input)
        {
            if (!(input is ValueContainerBase))
            {
                return input;
            }
            
            return input switch
            {
                Int32Container container => container.Value,
                Int16Container container => container.Value,
                Int8Container container => container.Value,
                DecimalContainer container => container.Value,
                FloatContainer container => container.Value,
                DoubleContainer container => container.Value,
                _ => input
            };
        }
    }
}