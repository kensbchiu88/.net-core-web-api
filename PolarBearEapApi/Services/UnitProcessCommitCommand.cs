using FIT.MES.Service;
using Newtonsoft.Json;
using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class UnitProcessCommitCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "UNIT_PROCESS_COMMIT";

        private readonly ILogger<UnitProcessCommitCommand> _logger;

        public UnitProcessCommitCommand(ILogger<UnitProcessCommitCommand> logger) => _logger = logger;
        MesCommandResponse IMesCommand.Execute(string serializedData)
        {
            EquipmentService service = new EquipmentService();

            string lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");

            try
            {
                string mesReturn = service.UNIT_PROCESS_COMMIT(lineCode, sectionCode, sn);
                return new MesCommandResponse(mesReturn);
            } 
            catch (Exception ex) 
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(serializedData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(serializedData, ex.Message));
                return MesCommandResponse.CallMesServiceException();
            }
        }
    }
}
