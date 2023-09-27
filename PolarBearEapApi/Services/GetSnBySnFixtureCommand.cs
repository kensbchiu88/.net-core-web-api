using FIT.MES.Service;
using Newtonsoft.Json;
using PolarBearEapApi.Commons;
using System.Diagnostics;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class GetSnBySnFixtureCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "GET_SN_BY_SN_FIXTURE";

        private readonly ILogger<GetSnBySnFixtureCommand> _logger;

        public GetSnBySnFixtureCommand(ILogger<GetSnBySnFixtureCommand> logger) => _logger = logger;

        MesCommandResponse IMesCommand.Execute(string serializedData)
        {
            EquipmentService _Service = new EquipmentService();

            string refValue = JsonUtil.GetParameter(serializedData, "OPRequestInfo.REF_VALUE");
            string refType = JsonUtil.GetParameter(serializedData, "OPRequestInfo.REF_VALUE");

            try 
            {
                string mesReturn = _Service.GET_SN_BY_SN_FIXTURE(refValue);
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
