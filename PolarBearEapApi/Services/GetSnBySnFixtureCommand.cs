using Newtonsoft.Json;
using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class GetSnBySnFixtureCommand : IMesCommand
    {
        public string CommandName { get; } = "GET_SN_BY_SN_FIXTURE";

        private readonly ILogger<GetSnBySnFixtureCommand> _logger;
        private readonly IMesService _equipmentService;

        public GetSnBySnFixtureCommand(ILogger<GetSnBySnFixtureCommand> logger, IMesService equipmentService) 
        {
            _logger = logger;
            _equipmentService = equipmentService;
        }

        public MesCommandResponse Execute(MesCommandRequest input)
        {
            
            string? refValue = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.REF_VALUE");

            try 
            {
                string mesReturn = _equipmentService.GET_SN_BY_SN_FIXTURE(refValue);
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
                    //response.OpResponseInfo = "{\"Result\":\"NG\"}";
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
    }


}
