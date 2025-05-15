using System;

namespace LoggerDiagram.PlcException
{
    public class PlcDataReadException : Exception
    {
        public PlcDataReadException(string message, Exception innerException) : base(message, innerException) { }
    }
}
