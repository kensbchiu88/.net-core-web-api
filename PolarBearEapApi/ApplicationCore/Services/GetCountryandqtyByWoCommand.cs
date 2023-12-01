using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** GET_COUNTRYANDQTY_BY_WO 根據工單、類型獲取國別/數量SN Not Ready*/
    public class GetCountryandqtyByWo : IMesCommand
    {
        public string CommandName { get; } = "GET_COUNTRYANDQTY_BY_WO";

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? wo = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.WO");
            string? classify = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.Classify");

            /*
            try
            {
                string mesReturn = await _equipmentService.GET_COUNTRYANDQTY_BY_WO(lineCode!, sectionCode!, stationCode!, wo!, sn!, token.username!);
                return new MesCommandResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
            */
            var reponse = new MesCommandResponse 
            {
                OpResponseInfo = "{\"Datas\":\"\"}",
                ErrorMessage = "MES not ready"
            };
            return reponse;
        }

        private static void ValidateInput(string serializedData)
        {
            List<string> requiredFields = new List<string>();

            string? lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(serializedData, "StationCode");
            string? wo = JsonUtil.GetParameter(serializedData, "OPRequestInfo.WO");
            string? classify = JsonUtil.GetParameter(serializedData, "OPRequestInfo.Classify");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(wo))
                requiredFields.Add("OPRequestInfo.WO");
            if (string.IsNullOrEmpty(classify))
                requiredFields.Add("OPRequestInfo.Classify");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }
    }
}
