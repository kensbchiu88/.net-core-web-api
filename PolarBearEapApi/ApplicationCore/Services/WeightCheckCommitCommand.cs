using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** WEIGHT_CHECK_COMMIT Not Ready*/
    public class WeightCheckCommitCommand : IMesCommand
    {
        public string CommandName { get; } = "WEIGHT_CHECK_COMMIT";

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");
            string? packType = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.PACK_TYPE");
            string? weight = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.WEIGHT");

            /*
            try
            {
                string mesReturn = await _equipmentService.WEIGHT_CHECK_COMMIT(lineCode!, sectionCode!, stationCode!, wo!, sn!, token.username!);
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
            string? sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");
            string? packType = JsonUtil.GetParameter(serializedData, "OPRequestInfo.PACK_TYPE");
            string? weight = JsonUtil.GetParameter(serializedData, "OPRequestInfo.WEIGHT");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(sn))
                requiredFields.Add("OPRequestInfo.SN");
            if (string.IsNullOrEmpty(packType))
                requiredFields.Add("OPRequestInfo.PACK_TYPE");
            if (string.IsNullOrEmpty(weight))
                requiredFields.Add("OPRequestInfo.WEIGHT");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }
    }
}
