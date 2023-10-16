using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class UnitProcessCommitCommand : IMesCommand
    {
        public string CommandName { get; } = "UNIT_PROCESS_COMMIT";

        private readonly ILogger<UnitProcessCommitCommand> _logger;
        private readonly IMesService _equipmentService;

        const string _TEST_FAIL = "FAIL";

        public UnitProcessCommitCommand(ILogger<UnitProcessCommitCommand> logger, IMesService equipmentService)
        {
            _logger = logger;
            _equipmentService = equipmentService;
        }
        public MesCommandResponse Execute(MesCommandRequest input)
        {
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
                    string mesReturn = _equipmentService.UNIT_PROCESS_COMMIT(lineCode, sectionCode, stationCode, sn, _TEST_FAIL, errorcode, errorMessage);
                    response = new MesCommandResponse(mesReturn);
                }
                else
                {
                    string mesReturn = _equipmentService.UNIT_PROCESS_COMMIT(lineCode, sectionCode, stationCode, sn);
                    response = new MesCommandResponse(mesReturn);
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
