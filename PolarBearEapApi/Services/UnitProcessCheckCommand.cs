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
        public UnitProcessCheckCommand(ILogger<UnitProcessCheckCommand> logger) => _logger = logger;

        MesCommandResponse IMesCommand.Execute(string serializedData)
        {
            EquipmentService service = new EquipmentService();

            string stationCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");

            try 
            {
                string mesReturn = service.UNIT_PROCESS_CHECK(sn, stationCode);
                return new MesCommandResponse(mesReturn);
            } catch (Exception ex) 
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(serializedData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(serializedData, ex.Message));
                return MesCommandResponse.CallMesServiceException();
            }
        }

    }
}
