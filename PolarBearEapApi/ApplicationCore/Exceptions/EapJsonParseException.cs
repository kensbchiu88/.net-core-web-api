using System;

namespace PolarBearEapApi.ApplicationCore.Exceptions
{
    public class EapJsonParseException : Exception
    {
        public EapJsonParseException()
        {
        }

        public EapJsonParseException(string message)
            : base(message)
        {
        }

        public EapJsonParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
