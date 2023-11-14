using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class GetCountryPnCheckResultCommand : IMesCommand
    {
        public string CommandName { get; } = "GET_COUNTRY_PN_CHECK_RESULT";

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? pnCode = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.PN_CODE");
            string? countryCode = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.COUNTRY_CODE");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");

            /*
            try
            {
                string mesReturn = await _equipmentService.GET_COUNTRY_PN_CHECK_RESULT(lineCode!, sectionCode!, stationCode!, wo!, sn!, token.username!);
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
            string? pnCode = JsonUtil.GetParameter(serializedData, "OPRequestInfo.PN_CODE");
            string? countryCode = JsonUtil.GetParameter(serializedData, "OPRequestInfo.COUNTRY_CODE");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(sn))
                requiredFields.Add("OPRequestInfo.SN");
            if (string.IsNullOrEmpty(pnCode))
                requiredFields.Add("OPRequestInfo.PN_CODE");
            if (string.IsNullOrEmpty(countryCode))
                requiredFields.Add("OPRequestInfo.COUNTRY_CODE");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }
    }
}
