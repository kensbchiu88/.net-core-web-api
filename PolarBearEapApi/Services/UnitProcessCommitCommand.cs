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
        const string _TEST_FAIL = "FAIL";

        public UnitProcessCommitCommand(ILogger<UnitProcessCommitCommand> logger) => _logger = logger;
        MesCommandResponse IMesCommand.Execute(MesCommandRequest input)
        {
            EquipmentService service = new EquipmentService();

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");
            string? result = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.result");

            try
            {
                MesCommandResponse response;
                if (!string.IsNullOrEmpty(result))
                {
                    string? errorcode = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.list_of_failing_tests");
                    string? errorMessage = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.failure_message");
                    string mesReturn = service.UNIT_PROCESS_COMMIT(lineCode, sectionCode, stationCode,sn, _TEST_FAIL, errorcode, errorMessage);
                    response = new MesCommandResponse(mesReturn);
                } 
                else
                {
                    string mesReturn = service.UNIT_PROCESS_COMMIT(lineCode, sectionCode, stationCode, sn);
                    response =  new MesCommandResponse(mesReturn);
                }
                return response;
            } 
            catch (Exception ex) 
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.Message));
                return MesCommandResponse.Fail(ErrorCodeEnum.CallMesServiceException);
            }
        }
    }
}
