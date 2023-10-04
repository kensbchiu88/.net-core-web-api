namespace PolarBearEapApi.Commons.Exceptions
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message)
   : base(message)
        {
        }

        public InvalidTokenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
