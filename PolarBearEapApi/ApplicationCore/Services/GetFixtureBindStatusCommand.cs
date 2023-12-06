using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class GetFixtureBindStatusCommand : IMesCommand
    {
        public string CommandName { get; } = "GET_FIXTURE_BIND_STATUS";

        private readonly IMesService _equipmentService;

        private const string OK_STRING = "UC IS CURRENT USED";

        public GetFixtureBindStatusCommand(IMesService equipmentService) 
        {
            _equipmentService = equipmentService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            SerializeDataModel inputModel = JsonConvert.DeserializeObject<SerializeDataModel>(input.SerializeData);
            ValidateInput(inputModel);

            try
            {
                string mesReturn = await _equipmentService.CHECK_UC_STATUS(inputModel.OPRequestInfo.FixtureSn!);
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
                if (fitMesResponse.IsResultOk())
                {
                    if (OK_STRING.Equals(fitMesResponse.ResultCode, StringComparison.OrdinalIgnoreCase))
                    {
                        response.OpResponseInfo = "{\"Result\":\"OK\"}";
                    }
                    else
                    {
                        response.OpResponseInfo = "{\"Result\":\"NG\"}";
                    }
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

        private static void ValidateInput(SerializeDataModel inputModel)
        {
            List<string> requiredFields = new List<string>();

            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.FixtureSn))
                requiredFields.Add("OPRequestInfo.FIXTURE_SN");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }

        private class OpRequestInfoModel
        {
            [JsonProperty("FIXTURE_SN")]
            public string? FixtureSn { get; set; }
        }

        private class SerializeDataModel
        {
            public string? LineCode { get; set; }
            public string? SectionCode { get; set; }
            public int? StationCode { get; set; }
            [Required]
            public OpRequestInfoModel? OPRequestInfo { get; set; }
        }
    }
}
