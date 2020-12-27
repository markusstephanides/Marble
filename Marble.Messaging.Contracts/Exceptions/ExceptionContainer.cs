using System;

namespace Marble.Messaging.Contracts.Exceptions
{
    public class ExceptionContainer : Exception
    {
        public ExceptionContainer(string message, string exceptionType) : base(message)
        {
            this.ExceptionType = exceptionType;
        }

        public string ExceptionType { get; }
    }
}