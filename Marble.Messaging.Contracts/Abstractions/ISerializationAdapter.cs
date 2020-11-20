using System;

namespace Marble.Messaging.Contracts.Abstractions
{
    public interface ISerializationAdapter
    {
        byte[] Serialize(object obj);
        object? Deserialize(byte[] bytes, Type type);
        T Deserialize<T>(byte[] bytes);
    }
}