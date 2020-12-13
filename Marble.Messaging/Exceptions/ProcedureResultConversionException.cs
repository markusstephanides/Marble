using System;

namespace Marble.Messaging.Exceptions
{
    public class ProcedureResultConversionException : Exception
    {
        public ProcedureResultConversionException(string message, Exception innerException) : base(message,
            innerException)
        {
        }
    }
}