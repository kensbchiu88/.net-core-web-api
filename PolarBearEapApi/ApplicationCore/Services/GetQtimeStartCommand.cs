using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** GET_QTIME_START 抓取前一站膠水的OPEN TIME */
    public class GetQtimeStartCommand : IMesCommand
    {
        public string CommandName { get; } = "GET_QTIME_START";
        private readonly IMesService _equipmentService;

        public GetQtimeStartCommand(IMesService equipmentService) 
        { 
            _equipmentService = equipmentService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");
            string? preSectionCode = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.PRE_SECTION_CODE");
            string? preStationCode = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.PRE_STATION_CODE");

            try
            {
                string mesReturn = await _equipmentService.GET_QTIME_START(sn!, preSectionCode!, preStationCode!);
                return GetResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
        }

        private static void ValidateInput(string serializedData)
        {
            List<string> requiredFields = new List<string>();

            string? sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");
            string? preSectionCode = JsonUtil.GetParameter(serializedData, "OPRequestInfo.PRE_SECTION_CODE");
            string? preStationCode = JsonUtil.GetParameter(serializedData, "OPRequestInfo.PRE_STATION_CODE");

            if (string.IsNullOrEmpty(sn))
                requiredFields.Add("OPRequestInfo.SN");
            if (string.IsNullOrEmpty(preSectionCode))
                requiredFields.Add("OPRequestInfo.PreSectionCode");
            if (string.IsNullOrEmpty(preStationCode))
                requiredFields.Add("OPRequestInfo.PreStationCode");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }

        private static MesCommandResponse GetResponse(string mesReturn)
        {
            var response = new MesCommandResponse();
            var fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturn);
            if (fitMesResponse != null)
            {
                if (fitMesResponse.Result != null && "OK".Equals(fitMesResponse.Result.ToUpper()))
                {
                    response.OpResponseInfo = "{\"Data\":\"" + fitMesResponse.ResultCode + "\"}"; ;
                }
                else
                {
                    response.OpResponseInfo = "{\"Data\":\"\"}";
                    response.ErrorMessage = fitMesResponse.Display;
                }
            }
            else
            {
                response.OpResponseInfo = "{\"Data\":\"\"}";
                response.ErrorMessage = ErrorCodeEnum.NoMesReturn.ToString();
            }

            return response;
        }
    }
}
