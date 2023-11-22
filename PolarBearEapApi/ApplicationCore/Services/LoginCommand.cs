using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.PublicApi.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class LoginCommand : IMesCommand
    {
        public string CommandName { get; } = "LOGIN";

        private readonly ITokenRepository _tokenService;
        private readonly ILogger<LoginCommand> _logger;
        private readonly IMesService _equipmentService;

        public LoginCommand(ITokenRepository tokenService, ILogger<LoginCommand> logger, IMesService equipmentService)
        {
            _tokenService = tokenService;
            _logger = logger;
            _equipmentService = equipmentService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            SerializeDataModel inputModel = JsonConvert.DeserializeObject<SerializeDataModel>(input.SerializeData);
            ValidateInput(inputModel);

            string? mesReturn;
            //驗證帳密
            try
            {
                mesReturn = await _equipmentService.CHECK_OP_PASSWORD(inputModel.OPRequestInfo!.User!, inputModel.OPRequestInfo!.Pwd!);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.ToString()));
                return MesCommandResponse.Fail(ErrorCodeEnum.CallMesServiceException);
            }

            //驗證MES結果
            if (!FITMesResponse.IsResultOk(mesReturn))
            {
                return Fail(ErrorCodeEnum.LoginFail);
            }

            //產生token
            try
            {
                string id = await _tokenService.Create(inputModel.OPRequestInfo!.User!);
                return Success(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.ToString()));
                return Fail(ErrorCodeEnum.SaveTokenFail);
            }
        }

        //return example : OPResponseInfo\":{\"Hwd\":\"36bd2cd3-3c94-4d53-a494-79eab5d34e9f\"}
        private MesCommandResponse Success(string token)
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Hwd\":\"" + token + "\"}";
            return response;
        }

        private MesCommandResponse Fail(ErrorCodeEnum errorCodeEnum)
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Hwd\":\"\"}";
            response.ErrorMessage = errorCodeEnum.ToString();
            return response;
        }

        private static void ValidateInput(SerializeDataModel inputModel)
        {
            List<string> requiredFields = new List<string>();

            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.User))
                requiredFields.Add("OPRequestInfo.user");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.Pwd))
                requiredFields.Add("OPRequestInfo.pwd");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }

        private class OpRequestInfoModel
        {
            public string? User { get; set; }
            public string? Pwd { get; set; }
        }

        private class SerializeDataModel
        {
            [Required]
            public OpRequestInfoModel? OPRequestInfo { get; set; }
        }
    }
}
