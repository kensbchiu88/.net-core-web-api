using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class GetRemainingOpentimeCommand : IMesCommand
    {
        public string CommandName { get; } = "GET_REMAINING_OPENTIME";

        private readonly IEquipmentTemporaryDataRepository _repository;
        private readonly ILogger<SetRemainingOpentimeCommand> _logger;
        private const string KEY = "REMAINING_OPENTIME";

        public GetRemainingOpentimeCommand(IEquipmentTemporaryDataRepository repository, ILogger<SetRemainingOpentimeCommand> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");

            try
            {
                var entity = await _repository.GetOne(lineCode!, sn!, KEY);
                if (entity != null)
                {
                    return Success(entity.DataValue);
                }
                else
                {
                    return NoDataFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, e.ToString()));
                return Fail(ErrorCodeEnum.QueryDbError);
            }
        }

        private static void ValidateInput(string serializedData)
        {
            List<string> requiredFields = new List<string>();

            string? lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(serializedData, "StationCode");
            string? sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(sn))
                requiredFields.Add("OPRequestInfo.SN");
        }

        private MesCommandResponse Success(string data)
        {           
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Data\": \"" + data + "\"}";
            return response;
        }

        /** Fail. return {\"Data\":\"\"} */
        private MesCommandResponse Fail(ErrorCodeEnum errorCodeEnum)
        {
            MesCommandResponse response = new MesCommandResponse();
            response.ErrorMessage = errorCodeEnum.ToString();
            response.OpResponseInfo = "{\"Data\":\"\"}";
            return response;
        }

        private MesCommandResponse NoDataFound()
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Data\":\"\"}";
            response.ErrorMessage = ErrorCodeEnum.NoDataFound.ToString();
            return response;
        }
    }
}
