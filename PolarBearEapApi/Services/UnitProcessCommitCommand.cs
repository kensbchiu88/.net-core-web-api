using FIT.MES.Service;
using Newtonsoft.Json;
using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class UnitProcessCommitCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "UNIT_PROCESS_COMMIT";
        MesCommandResponse IMesCommand.Execute(string serializedData)
        {
            EquipmentService service = new EquipmentService();

            string lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");

            string mesReturn = service.UNIT_PROCESS_COMMIT(lineCode, sectionCode, sn);
            //return GetResponse(mesReturn);
            return new MesCommandResponse(mesReturn);
        }

        /*
        MesCommandResponse response = new MesCommandResponse();
        response.OpResponseInfo = "{\"Result\":\"OK\"}";
        return response;
        */
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
