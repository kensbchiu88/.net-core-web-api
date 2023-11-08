using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class SnLinkWoCommand : IMesCommand
    {
        public string CommandName { get; } = "SN_LINK_WO";

        private readonly IMesService _equipmentService;
        private readonly ITokenRepository _tokenRepository;

        public SnLinkWoCommand(IMesService equipmentService, ITokenRepository tokenRepository)
        {
            _equipmentService = equipmentService;
            _tokenRepository = tokenRepository;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");
            string? wo = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.WO");

            TokenInfo token = await _tokenRepository.GetTokenInfo(input.Hwd);

            try
            {
                string mesReturn = await _equipmentService.SN_LINK_WO(lineCode!, sectionCode!, stationCode!, wo!, sn!, token.username!);
                return new MesCommandResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
        }

        private static void ValidateInput(string serializedData)
        {
            List<string> requiredFields = new List<string>();

            string? lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(serializedData, "StationCode");
            string? sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");
            string? wo = JsonUtil.GetParameter(serializedData, "OPRequestInfo.WO");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(sn))
                requiredFields.Add("OPRequestInfo.SN");
            if (string.IsNullOrEmpty(wo))
                requiredFields.Add("OPRequestInfo.WO");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }
    }
}
