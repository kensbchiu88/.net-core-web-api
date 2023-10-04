using static PolarBearEapApi.Services.IMesCommand;
using PolarBearEapApi.Models;
using PolarBearEapApi.Commons;
using FIT.MES.Service;
using Newtonsoft.Json;

namespace PolarBearEapApi.Services
{
    public class AddBomDataCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "ADD_BOM_DATA";

        private readonly ILogger<AddBomDataCommand> _logger;

        public AddBomDataCommand(ILogger<AddBomDataCommand> logger) => _logger = logger;

        MesCommandResponse IMesCommand.Execute(MesCommandRequest input)
        {
            EquipmentService service = new EquipmentService();

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");
            string? rawSn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.RAW_SN");

            try
            {
                string mesReturn = service.ADD_BOM_DATA(lineCode, sectionCode, stationCode, sn, rawSn);
                //return new MesCommandResponse(mesReturn, "{\"Result\":\"OK\",\"ResultCoded\":\"DEFAULT\"}");
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
