namespace PolarBearEapApi.Commons.Exceptions
{
    public class TokenExpireException : Exception
    {
        public TokenExpireException(string message)
           : base(message)
        {
        }

        public TokenExpireException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
