using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class BindCommand : IMesCommand
    {
        public string CommandName { get; } = "BIND";

        private readonly ITokenService _tokenService;
        private readonly ILogger<BindCommand> _logger;
        private readonly IMesService _equipmentService;

        public BindCommand(ITokenService context, ILogger<BindCommand> logger, IMesService equipmentService)
        {
            _tokenService = context;
            _logger = logger;
            _equipmentService = equipmentService;
        }

        public MesCommandResponse Execute(MesCommandRequest input)
        {
            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? serverVersion = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.ServerVersion");

            //Todo : call mes 檢查職能
            TokenInfo tokenInfo = _tokenService.GetTokenInfo(input.Hwd);

        
            string? mesReturn;
            try
            {
                mesReturn = _equipmentService.CHECK_SECTION_PERMISSION(tokenInfo.username, sectionCode, stationCode);
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
            _tokenService.BindMachine(input.Hwd, lineCode, sectionCode, stationCode, serverVersion);

            return MesCommandResponse.Ok();
        }
    }
}
