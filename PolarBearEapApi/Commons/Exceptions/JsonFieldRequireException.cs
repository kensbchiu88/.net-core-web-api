namespace PolarBearEapApi.Commons.Exceptions
{
    public class JsonFieldRequireException : Exception
    {
        public JsonFieldRequireException()
        {
        }

        public JsonFieldRequireException(string message)
            : base(message)
        {
        }

        public JsonFieldRequireException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
