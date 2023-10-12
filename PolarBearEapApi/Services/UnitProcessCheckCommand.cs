using FIT.MES.Service;
using Newtonsoft.Json;
using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class UnitProcessCheckCommand : IMesCommand
    {
        public string CommandName { get; } = "UNIT_PROCESS_CHECK";

        private readonly ILogger<UnitProcessCheckCommand> _logger;
        private readonly IMesService _equipmentService;
        public UnitProcessCheckCommand(ILogger<UnitProcessCheckCommand> logger, IMesService equipmentService) 
        { 
            _logger = logger;
            _equipmentService = equipmentService;
        }

        public MesCommandResponse Execute(MesCommandRequest input)
        {
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");

            try 
            {
                string mesReturn = _equipmentService.UNIT_PROCESS_CHECK(sn, sectionCode, stationCode);
                return new MesCommandResponse(mesReturn);
            } catch (Exception ex) 
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.Message));
                return MesCommandResponse.Fail(ErrorCodeEnum.CallMesServiceException);
            }
        }

    }
}
