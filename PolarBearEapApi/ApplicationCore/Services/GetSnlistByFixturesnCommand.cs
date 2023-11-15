using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class GetSnlistByFixturesnCommand : IMesCommand
    {
        public string CommandName { get; } = "GET_SNLIST_BY_FIXTURESN";

        private readonly IMesService _equipmentService;

        public GetSnlistByFixturesnCommand(IMesService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? refValue = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.REF_VALUE");

            try
            {
                string mesReturn = await _equipmentService.GET_SNLIST_BY_FIXTURESN(sectionCode!, stationCode!, refValue!);
                return GetResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
        }

        private static MesCommandResponse GetResponse(string mesReturn)
        {
            var response = new MesCommandResponse();
            var fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturn);
            if (fitMesResponse != null)
            {
                if (fitMesResponse.Result != null && "OK".Equals(fitMesResponse.Result.ToUpper()))
                {
                    response.OpResponseInfo = "{\"SN\":\"" + fitMesResponse.ResultCode + "\"}"; ;
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

        private static void ValidateInput(string serializedData)
        {
            List<string> requiredFields = new List<string>();

            string? sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(serializedData, "StationCode");
            string? refValue = JsonUtil.GetParameter(serializedData, "OPRequestInfo.REF_VALUE");



            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(refValue))
                requiredFields.Add("OPRequestInfo.REF_VALUE");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }
    }
}
