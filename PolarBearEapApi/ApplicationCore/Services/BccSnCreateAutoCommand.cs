using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class BccSnCreateAutoCommand : IMesCommand

    {
        public string CommandName { get; } = "BCC_SN_CREATE_AUTO";

        private readonly IMesService _equipmentService;
        private readonly ITokenRepository _tokenRepository;

        public BccSnCreateAutoCommand(IMesService equipmentService, ITokenRepository tokenRepository)
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
                string mesReturn = await _equipmentService.BCC_SN_CREATE_AUTO(inputModel.OPRequestInfo!.WorkOrder!, inputModel.OPRequestInfo.PrintNo ?? 0, token.username!);
                return GetResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
        }

        private static void ValidateInput(SerializeDataModel inputModel)
        {
            List<string> requiredFields = new List<string>();

            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.WorkOrder))
                requiredFields.Add("OPRequestInfo.WORK_ORDER");
            if (inputModel.OPRequestInfo.PrintNo == null)
                requiredFields.Add("OPRequestInfo.PRINT_NO");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
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

        private class OpRequestInfoModel
        {
            [JsonProperty("WORK_ORDER")]
            public string? WorkOrder { get; set; }
            [JsonProperty("LABLE_CODE")]
            public string? LabelCode { get; set; }
            [JsonProperty("PRINT_NO")]
            public int? PrintNo { get; set; }
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
