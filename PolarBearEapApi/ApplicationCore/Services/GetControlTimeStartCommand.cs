using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** GET_CONTROL_TIME_START Not Ready */
    public class GetControlTimeStartCommand : IMesCommand
    {
        public string CommandName { get; } = "GET_CONTROL_TIME_START";

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? refData = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.REF_DATA");

            /*
            try
            {
                string mesReturn = await _equipmentService.GET_CONTROL_TIME_START(lineCode!, sectionCode!, stationCode!, wo!, sn!, token.username!);
                return new MesCommandResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
            */
            MesCommandResponse response = new MesCommandResponse
            {
                OpResponseInfo = "{\"Result\":\"NG\"}",
                ErrorMessage = "MES not ready"
            };

            return response;
        }

        private static void ValidateInput(string serializedData)
        {
            List<string> requiredFields = new List<string>();

            string? lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(serializedData, "StationCode");
            string? refData = JsonUtil.GetParameter(serializedData, "OPRequestInfo.REF_DATA");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(refData))
                requiredFields.Add("OPRequestInfo.REF_DATA");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }
    }
}
