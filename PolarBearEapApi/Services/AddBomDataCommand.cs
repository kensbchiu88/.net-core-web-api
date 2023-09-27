using static PolarBearEapApi.Services.IMesCommand;
using PolarBearEapApi.Models;
using PolarBearEapApi.Commons;
using FIT.MES.Service;
using Newtonsoft.Json;

namespace PolarBearEapApi.Services
{
    public class AddBomDataCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "ADD_BOM_DATA";

        MesCommandResponse IMesCommand.Execute(string serializedData)
        {
            EquipmentService service = new EquipmentService();

            string lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");
            string rawSn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.RAW_SN");

            string mesReturn = service.ADD_BOM_DATA(lineCode, sectionCode, sn, rawSn);
            return new MesCommandResponse(mesReturn, "{\"Result\":\"OK\",\"ResultCoded\":\"DEFAULT\"}");
        }


        /*
        MesCommandResponse response = new MesCommandResponse();
        response.OpResponseInfo = "{\"Result\":\"OK\",\"ResultCoded\":\"DEFAULT\"}";
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
                    response.OpResponseInfo = "{\"Result\":\"OK\",\"ResultCoded\":\"DEFAULT\"}";
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
