﻿using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;
using PolarBearEapApi.Commons.Exceptions;
using FIT.MES.Service;
using Newtonsoft.Json;

namespace PolarBearEapApi.Services
{
    public class LoginCommand : IMesCommand
    {
        public string CommandName { get; } = "LOGIN";

        private readonly ITokenService _tokenService;
        private readonly ILogger<LoginCommand> _logger;

        public LoginCommand(ITokenService tokenService, ILogger<LoginCommand> logger) 
        {
            _tokenService = tokenService;
            _logger = logger; 
        }

        public MesCommandResponse Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string username = JsonUtil.GetCaseSensitiveParameter(input.SerializeData, "OPRequestInfo.user")!;
            string password = JsonUtil.GetCaseSensitiveParameter(input.SerializeData, "OPRequestInfo.pwd")!;

            //string fitMesResult = "NG";

            string? mesReturn;
            FITMesResponse? fitMesResponse;
            //驗證帳密
            try 
            {
                EquipmentService service = new EquipmentService();
                mesReturn = service.CHECK_OP_PASSWORD(username, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.Message));
                return MesCommandResponse.Fail(ErrorCodeEnum.CallMesServiceException);
            }

            //驗證MES結果
            fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturn);
            if (fitMesResponse == null || string.IsNullOrEmpty(fitMesResponse.Result) || !fitMesResponse.Result.ToUpper().Equals("OK"))
            { 
                return MesCommandResponse.Fail(ErrorCodeEnum.LoginFail);
            }

            //產生token
            try 
            {
                string id = _tokenService.Create(username);
                return Success(id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.Message));
                return MesCommandResponse.Fail(ErrorCodeEnum.SaveTokenFail);
            }
        }

        //return example : OPResponseInfo\":{\"Hwd\":\"36bd2cd3-3c94-4d53-a494-79eab5d34e9f\"}
        private MesCommandResponse Success(String token)
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Hwd\":\"" + token + "\"}";
            return response;
        }

        private MesCommandResponse Fail(ErrorCodeEnum errorCodeEnum)
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"NG\"}";
            response.ErrorMessage = errorCodeEnum.ToString();
            return response;
        }

        private static void ValidateInput(string serializedData) 
        {
            string? username = JsonUtil.GetCaseSensitiveParameter(serializedData, "OPRequestInfo.user");
            string? password = JsonUtil.GetCaseSensitiveParameter(serializedData, "OPRequestInfo.pwd");

            if (string.IsNullOrEmpty(username) )
            {
                throw new JsonFieldRequireException("user field is required");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new JsonFieldRequireException("pwd field is required");
            }
        }
    }
}
