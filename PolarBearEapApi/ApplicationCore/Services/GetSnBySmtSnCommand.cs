using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class GetSnBySmtSnCommand : IMesCommand
    {
        public string CommandName { get; } = "GET_SN_BY_SMTSN";

        private readonly ILogger<GetSnBySmtSnCommand> _logger;
        private readonly IMesService _equipmentService;

        public GetSnBySmtSnCommand(ILogger<GetSnBySmtSnCommand> logger, IMesService equipmentService)
        {
            _logger = logger;
            _equipmentService = equipmentService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            string? refValue = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.REF_VALUE");
            if(string.IsNullOrEmpty(refValue)) 
            {
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "OPRequestInfo.REF_VALUE is required");
            }

            try
            {
                string mesReturn = await _equipmentService.GET_SN_BY_SMTSN(refValue!);
                return GetResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
        }

        private static MesCommandResponse GetResponse(string mesReturn)
        {
            var response = new MesCommandResponse();
            var fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturn);
            if (fitMesResponse != null)
            {
                if (fitMesResponse.Result != null && "OK".Equals(fitMesResponse.Result.ToUpper()))
                {
                    response.OpResponseInfo = "{\"SN\":\"" + fitMesResponse.ResultCode + "\"}"; ;
                }
                else
                {
                    response.OpResponseInfo = "{\"SN\":\"\"}";
                    response.ErrorMessage = fitMesResponse.Display;
                }
            }
            else
            {
                response.OpResponseInfo = "{\"SN\":\"\"}";
                response.ErrorMessage = ErrorCodeEnum.NoMesReturn.ToString();
            }

            return response;
        }

        private static MesCommandResponse FailResponse(ErrorCodeEnum errorCodeEnum)
        {
            var response = new MesCommandResponse();
            response.OpResponseInfo = "{\"SN\":\"\"}";
            response.ErrorMessage = errorCodeEnum.ToString();
            return response;
        }
    }
}
