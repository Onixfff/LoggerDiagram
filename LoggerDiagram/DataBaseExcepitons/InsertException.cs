using System;

namespace LoggerDiagram.DataBaseExcepitons
{
    public class InsertException : Exception
    {
        public InsertException(string message, Exception innerException) :base(message, innerException) { }
    }
}
