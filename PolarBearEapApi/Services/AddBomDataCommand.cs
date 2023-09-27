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

        MesCommandResponse IMesCommand.Execute(string serializedData)
        {
            EquipmentService service = new EquipmentService();

            string lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");
            string rawSn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.RAW_SN");
            try
            {
                string mesReturn = service.ADD_BOM_DATA(lineCode, sectionCode, sn, rawSn);
                return new MesCommandResponse(mesReturn, "{\"Result\":\"OK\",\"ResultCoded\":\"DEFAULT\"}");
            } catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(serializedData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(serializedData, ex.Message));
                return MesCommandResponse.CallMesServiceException();
            }

        }
    }
}
