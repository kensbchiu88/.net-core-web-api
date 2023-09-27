using FIT.MES.Service;
using Newtonsoft.Json;
using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class UnitProcessCheckCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "UNIT_PROCESS_CHECK";
        MesCommandResponse IMesCommand.Execute(string serializedData)
        {
            EquipmentService service = new EquipmentService();

            string stationCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");

            string mesReturn = service.UNIT_PROCESS_CHECK(sn, stationCode);
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
                    response.OpResponseInfo = "{\"Result\":\"OK\"}";
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
        */
    }
}
