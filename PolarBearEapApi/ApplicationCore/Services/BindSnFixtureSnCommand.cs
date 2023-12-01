using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** BIND_SN_FIXTURESN 綁定產品SN與載具SN */
    public class BindSnFixtureSnCommand : IMesCommand
    {
        public string CommandName { get; } = "BIND_SN_FIXTURESN";

        private readonly IMesService _equipmentService;

        public BindSnFixtureSnCommand(IMesService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            //Newtonsoft does case insensitive JSON deserialization
            SerializeDataModel inputModel = JsonConvert.DeserializeObject<SerializeDataModel>(input.SerializeData);
            ValidateInput(inputModel);

            try
            {
                string mesReturn = await _equipmentService.BIND_FIXTURE_BY_SN_FIXTURE(inputModel.LineCode!, inputModel.SectionCode!, inputModel.StationCode.ToString()!, inputModel.OPRequestInfo.Sn!, inputModel.OPRequestInfo.FixtureSn!);
                return new MesCommandResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
        }

        private static void ValidateInput(SerializeDataModel inputModel)
        {
            List<string> requiredFields = new List<string>();

            if (string.IsNullOrEmpty(inputModel.LineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(inputModel.SectionCode))
                requiredFields.Add("SectionCode");
            if (inputModel.StationCode == null)
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.Sn))
                requiredFields.Add("OPRequestInfo.SN");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.FixtureSn))
                requiredFields.Add("OPRequestInfo.FIXTURE_SN");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }

        private class OpRequestInfoModel
        {
            [JsonProperty("SN")]
            public string? Sn { get; set; }
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
