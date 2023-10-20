namespace PolarBearEapApi.ApplicationCore.Exceptions
{
    public class InvalidTokenFormatException : Exception
    {
        public InvalidTokenFormatException(string message): base(message)
        {
        }

        public InvalidTokenFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
