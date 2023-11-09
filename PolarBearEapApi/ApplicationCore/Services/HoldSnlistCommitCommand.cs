using FIT.MES.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class HoldSnlistCommitCommand : IMesCommand
    {
        public string CommandName { get; } = "HOLD_SNLIST_COMMIT";

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? reason = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.HOLD_REASON");
            string? snListString = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN_LIST");

            var snList = JsonConvert.DeserializeObject<List<SnListModel>>(snListString);

            /*
            try
            {
                string mesReturn = await _equipmentService.UNBIND_SN_FIXTURESN(sn!);
                return new MesCommandResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
            */

            return new MesCommandResponse { OpResponseInfo = "{\"Result\":\"OK\"}"};
        }

        private static void ValidateInput(string serializedData)
        {
            List<string> requiredFields = new List<string>();

            string? lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(serializedData, "StationCode");
            string? reason = JsonUtil.GetParameter(serializedData, "OPRequestInfo.HOLD_REASON");
            string? snList = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN_LIST");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(reason))
                requiredFields.Add("OPRequestInfo.HOLD_REASON");
            if (string.IsNullOrEmpty(snList))
                requiredFields.Add("OPRequestInfo.SN_LIST");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }

        private class SnListModel
        {
            [JsonProperty("SN", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
            private string? Sn { get; set; }
        }
    }
}
