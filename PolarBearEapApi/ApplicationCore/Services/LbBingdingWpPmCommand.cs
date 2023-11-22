using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class LbBingdingWpPmCommand : IMesCommand
    {
        public string CommandName { get; } = "LB_BINGDING_WP_PM";

        private readonly IMesService _equipmentService;
        private readonly ITokenRepository _tokenRepository;

        public LbBingdingWpPmCommand(IMesService equipmentService, ITokenRepository tokenRepository)
        {
            _equipmentService = equipmentService;
            _tokenRepository = tokenRepository;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            SerializeDataModel inputModel = JsonConvert.DeserializeObject<SerializeDataModel>(input.SerializeData);
            ValidateInput(inputModel);

            TokenInfo token = await _tokenRepository.GetTokenInfo(input.Hwd);

            try
            {
                string mesReturn = await _equipmentService.LB_BINGDING_WP_PM(inputModel.OPRequestInfo!.WorkOrder!, inputModel.OPRequestInfo.PanelSn!, inputModel.OPRequestInfo.Sn!, inputModel.SectionCode!, inputModel.StationCode.ToString()!, token.username!);
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

            if (string.IsNullOrEmpty(inputModel.SectionCode))
                requiredFields.Add("SectionCode");
            if (inputModel.StationCode == null)
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.WorkOrder))
                requiredFields.Add("OPRequestInfo.WORK_ORDER");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.PanelSn))
                requiredFields.Add("OPRequestInfo.Panel_SN");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.Sn))
                requiredFields.Add("OPRequestInfo.SN");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }

        private class OpRequestInfoModel
        {
            [JsonProperty("WORK_ORDER")]
            public string? WorkOrder { get; set; }
            [JsonProperty("Panel_SN")]
            public string? PanelSn { get; set; }
            [JsonProperty("PRINT_NO")]
            public string? PrintNo { get; set; }
            [JsonProperty("SN")]
            public string? Sn { get; set; }
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
