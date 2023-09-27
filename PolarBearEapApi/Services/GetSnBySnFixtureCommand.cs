using FIT.MES.Service;
using Newtonsoft.Json;
using PolarBearEapApi.Commons;
using System.Diagnostics;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class GetSnBySnFixtureCommand : IMesCommand
    {
        private readonly ILogger<GetSnBySnFixtureCommand> _logger;

        string IMesCommand.CommandName { get; } = "GET_SN_BY_SN_FIXTURE";

        /*
        public GetSnBySnFixtureCommand()
        {
        }
        */

        public GetSnBySnFixtureCommand(ILogger<GetSnBySnFixtureCommand> logger) => _logger = logger;

        MesCommandResponse IMesCommand.Execute(string serializedData)
        {
            EquipmentService _Service = new EquipmentService();

            string refValue = JsonUtil.GetParameter(serializedData, "OPRequestInfo.REF_VALUE");
            string refType = JsonUtil.GetParameter(serializedData, "OPRequestInfo.REF_VALUE");

            string mesReturn = _Service.GET_SN_BY_SN_FIXTURE(refValue);

            Debug.WriteLine("REF_VALUE:" + refValue);
            Debug.WriteLine("REF_TYPE:" + refType);
            Debug.WriteLine("MES RETURN:" + mesReturn);

            return new MesCommandResponse(mesReturn);
        }

        /*
        private MesCommandResponse GetResponse(string mesReturnString)
        {
            MesCommandResponse response = new MesCommandResponse();
            var fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturnString);
            if (fitMesResponse != null)
            {
                if (fitMesResponse.Result != null && "OK".Equals(fitMesResponse.Result.ToUpper()))
                {
                    response.OpResponseInfo = "{\"SN\":\"" + fitMesResponse.ResultCode + "\"}";
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
        */
    }


}
