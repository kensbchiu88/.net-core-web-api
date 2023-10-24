using PolarBearEapApi.ApplicationCore.Constants;

namespace PolarBearEapApi.ApplicationCore.Exceptions
{
    /**
     * 共用的Exception，主要用來呈現預期中的檢核錯誤
     */
    public class EapException : Exception
    {
        public EapException(ErrorCodeEnum errorCodeEnum) : base(errorCodeEnum.ToString())
        { 
            
        }

        public EapException(ErrorCodeEnum errorCodeEnum, string message) : base(errorCodeEnum.ToString() + ":" + message)
        {
        }

        public EapException(ErrorCodeEnum errorCodeEnum, string message, Exception innerException) : base(errorCodeEnum.ToString() + ":" +  message, innerException)
        {
        }

        public EapException(ErrorCodeEnum errorCodeEnum, Exception innerException) : base(errorCodeEnum.ToString(), innerException)
        {

        }
    }
}
