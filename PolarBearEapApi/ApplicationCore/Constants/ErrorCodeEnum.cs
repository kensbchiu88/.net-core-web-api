namespace PolarBearEapApi.ApplicationCore.Constants
{
    public enum ErrorCodeEnum
    {
        ParseJsonError,
        QueryDbError,
        NoSuchCommand,
        NoDataFound,
        NoMesReturn,
        UploadFail,
        CallMesServiceException,
        SaveTokenFail,
        InvalidToken,
        TokenExpired,
        LoginFail,
        InvalidTokenFormat,
        JsonFieldRequire,
        InvalidStoredProcedureReturn,
    }
}
