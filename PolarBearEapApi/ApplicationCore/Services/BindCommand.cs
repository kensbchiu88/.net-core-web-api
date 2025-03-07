﻿using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** BIND 工位綁定 */
    public class BindCommand : IMesCommand
    {
        public string CommandName { get; } = "BIND";

        private readonly ITokenRepository _tokenService;
        private readonly ILogger<BindCommand> _logger;
        private readonly IMesService _equipmentService;

        public BindCommand(ITokenRepository context, ILogger<BindCommand> logger, IMesService equipmentService)
        {
            _tokenService = context;
            _logger = logger;
            _equipmentService = equipmentService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? serverVersion = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.ServerVersion");

            TokenInfo tokenInfo = await _tokenService.GetTokenInfo(input.Hwd);

            string? mesReturn;
            try
            {
                mesReturn = await _equipmentService.CHECK_SECTION_PERMISSION(tokenInfo.username, sectionCode, stationCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.Message));
                return MesCommandResponse.Fail(ErrorCodeEnum.CallMesServiceException);
            }

            if (!FITMesResponse.IsResultOk(mesReturn))
            {
                return new MesCommandResponse(mesReturn);
            }
            
            //Bind
            await _tokenService.BindMachine(input.Hwd, lineCode, sectionCode, stationCode, serverVersion);

            return MesCommandResponse.Ok();
        }
    }
}
