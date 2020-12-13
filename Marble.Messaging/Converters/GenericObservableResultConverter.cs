using System;
using System.Reactive.Linq;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;

namespace Marble.Messaging.Converters
{
    public class GenericObservableResultConverter : IResultConverter
    {
        public Type ConversionInType { get; set; } = typeof(IObservable<>);

        public MessageHandlingResult ConvertResult(object result, Type? genericTypeArgument = null!)
        {
            if (!genericTypeArgument!.IsValueType)
            {
                return new MessageHandlingResult
                {
                    ResultStream = (IObservable<object>) result
                };
            }

            return new MessageHandlingResult
            {
                ResultStream = this.ConvertValueTypeBasedObservableToObjectObservable(result, genericTypeArgument)
            };
        }

        private IObservable<object> ConvertValueTypeBasedObservableToObjectObservable(object result,
            Type genericTypeArgument)
        {
            if (genericTypeArgument == typeof(bool))
            {
                return ((IObservable<bool>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(byte))
            {
                return ((IObservable<byte>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(char))
            {
                return ((IObservable<char>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(decimal))
            {
                return ((IObservable<decimal>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(double))
            {
                return ((IObservable<double>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(float))
            {
                return ((IObservable<float>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(int))
            {
                return ((IObservable<int>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(long))
            {
                return ((IObservable<long>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(sbyte))
            {
                return ((IObservable<sbyte>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(short))
            {
                return ((IObservable<short>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(string))
            {
                return ((IObservable<string>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(uint))
            {
                return ((IObservable<uint>) result).Select(item => (object) item);
            }

            if (genericTypeArgument == typeof(ulong))
            {
                return ((IObservable<ulong>) result).Select(item => (object) item);
            }

            throw new Exception($"Invalid value type based IObservable provided: {genericTypeArgument.FullName}");
        }
    }
}