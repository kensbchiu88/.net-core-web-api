using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.Infra.Services;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class SpliteSnCommitCommand : IMesCommand
    {
        public string CommandName { get; } = "SPLITE_SN_COMMIT";

        private readonly IMesService _equipmentService;
        private readonly ITokenRepository _tokenRepository;

        public SpliteSnCommitCommand(IMesService equipmentService, ITokenRepository tokenRepository)
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
            string? wo = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.WO");
            string? parentSn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.PartentSn");
            string? childSnList = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.ChildSnList");
            string? carrierSn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.CarrierSn");

            TokenInfo token = await _tokenRepository.GetTokenInfo(input.Hwd);

            try
            {
                string mesReturn = await _equipmentService.SPLITE_SN_COMMIT(lineCode!, sectionCode!, stationCode!, wo!, parentSn!, childSnList!, carrierSn!, token.username!);
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
            string? wo = JsonUtil.GetParameter(serializedData, "OPRequestInfo.WO");
            string? parentSn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.PartentSn");
            string? childSnList = JsonUtil.GetParameter(serializedData, "OPRequestInfo.ChildSnList");
            string? carrierSn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.CarrierSn");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(wo))
                requiredFields.Add("OPRequestInfo.WO");
            if (string.IsNullOrEmpty(parentSn))
                requiredFields.Add("OPRequestInfo.PartentSn");
            if (string.IsNullOrEmpty(childSnList))
                requiredFields.Add("OPRequestInfo.ChildSnList");
            if (string.IsNullOrEmpty(carrierSn))
                requiredFields.Add("OPRequestInfo.CarrierSn");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }
    }
}
