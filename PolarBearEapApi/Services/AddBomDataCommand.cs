using static PolarBearEapApi.Services.IMesCommand;
using PolarBearEapApi.Models;
using PolarBearEapApi.Commons;
using FIT.MES.Service;
using Newtonsoft.Json;

namespace PolarBearEapApi.Services
{
    public class AddBomDataCommand : IMesCommand
    {
        public string CommandName { get; } = "ADD_BOM_DATA";

        private readonly ILogger<AddBomDataCommand> _logger;
        private readonly IMesService _equipmentService;

        public AddBomDataCommand(ILogger<AddBomDataCommand> logger, IMesService equipmentService) 
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
            string? rawSn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.RAW_SN");

            try
            {
                string mesReturn = _equipmentService.ADD_BOM_DATA(lineCode, sectionCode, stationCode, sn, rawSn);
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
