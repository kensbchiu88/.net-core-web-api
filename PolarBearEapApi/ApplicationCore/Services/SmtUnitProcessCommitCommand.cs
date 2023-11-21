using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class SmtUnitProcessCommitCommand : IMesCommand
    {
        public string CommandName { get; } = "SMT_UNIT_PROCESS_COMMIT";

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            SerializeDataModel inputModel = JsonConvert.DeserializeObject<SerializeDataModel>(input.SerializeData);
            ValidateInput(inputModel);

            MesCommandResponse response = new MesCommandResponse
            {
                OpResponseInfo = "{\"Result\":\"NG\"}",
                ErrorMessage = "MES not ready"
            };

            return response;
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
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.Result))
                requiredFields.Add("OPRequestInfo.Result");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.BinData))
                requiredFields.Add("OPRequestInfo.BinData");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.BadMark))
                requiredFields.Add("OPRequestInfo.BadMark");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }

        private class OpRequestInfoModel
        {
            public string? Sn { get; set; }
            public string? Result { get; set; }
            public string? BinData { get; set; }
            public string? BadMark { get; set; }
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
