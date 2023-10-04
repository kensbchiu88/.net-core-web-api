using FIT.MES.Service;
using Newtonsoft.Json;
using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class UnitProcessCheckCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "UNIT_PROCESS_CHECK";

        private readonly ILogger<UnitProcessCheckCommand> _logger;
        public UnitProcessCheckCommand(ILogger<UnitProcessCheckCommand> logger) 
        { 
            _logger = logger;
        }

        MesCommandResponse IMesCommand.Execute(MesCommandRequest input)
        {
            EquipmentService service = new EquipmentService();

            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");

            try 
            {
                string mesReturn = service.UNIT_PROCESS_CHECK(sn, sectionCode, stationCode);
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
