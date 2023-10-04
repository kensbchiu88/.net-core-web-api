using FIT.MES.Service;
using Newtonsoft.Json;
using PolarBearEapApi.Commons;
using System.Diagnostics;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class GetSnBySnFixtureCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "GET_SN_BY_SN_FIXTURE";

        private readonly ILogger<GetSnBySnFixtureCommand> _logger;

        public GetSnBySnFixtureCommand(ILogger<GetSnBySnFixtureCommand> logger) => _logger = logger;

        MesCommandResponse IMesCommand.Execute(MesCommandRequest input)
        {
            EquipmentService _Service = new EquipmentService();

            string? refValue = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.REF_VALUE");

            try 
            {
                string mesReturn = _Service.GET_SN_BY_SN_FIXTURE(refValue);
                return GetResponse(mesReturn);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.Message));
                return MesCommandResponse.Fail(ErrorCodeEnum.CallMesServiceException);
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
                    response.OpResponseInfo = "{\"Result\":\"NG\"}";
                    response.ErrorMessage = fitMesResponse.Display;
                }
            }
            else
            {
                response.OpResponseInfo = "{\"Result\":\"NG\"}";
                response.ErrorMessage = ErrorCodeEnum.NoMesReturn.ToString();
            }

            return response; 
        }
    }


}
