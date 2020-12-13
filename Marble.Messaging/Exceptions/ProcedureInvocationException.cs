using System;

namespace Marble.Messaging.Exceptions
{
    public class ProcedureInvocationException : Exception
    {
        public ProcedureInvocationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}