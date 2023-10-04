using System;

namespace PolarBearEapApi.Commons.Exceptions
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
