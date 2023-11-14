using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class SetRemainingOpentimeCommand : IMesCommand
    {
        public string CommandName { get; } = "SET_REMAINING_OPENTIME";

        private readonly IEquipmentTemporaryDataRepository _repository;
        private readonly ILogger<SetRemainingOpentimeCommand> _logger;
        private const string KEY = "REMAINING_OPENTIME";

        public SetRemainingOpentimeCommand(IEquipmentTemporaryDataRepository repository, ILogger<SetRemainingOpentimeCommand> logger)
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
            string? value = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.Data");

            try
            {
                //insert into db
                var entity = new EquipmentTemporaryDataEntity
                {
                    LineCode = lineCode!,
                    SectionCode = sectionCode!,
                    StationCode = int.Parse(stationCode!),
                    Sn = sn!,
                    DataKey = KEY,
                    DataValue = value!,
                    CreateTime = DateTime.Now
                };
                await _repository.Insert(entity);

                return MesCommandResponse.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.ToString()));
                return MesCommandResponse.Fail(ErrorCodeEnum.SetDataFail);
            }
        }

        private static void ValidateInput(string serializedData)
        {
            List<string> requiredFields = new List<string>();

            string? lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(serializedData, "StationCode");
            string? sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN");
            string? value = JsonUtil.GetParameter(serializedData, "OPRequestInfo.Data");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(sn))
                requiredFields.Add("OPRequestInfo.SN");
            if (string.IsNullOrEmpty(value))
                requiredFields.Add("OPRequestInfo.Data");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
            try 
            {
                DateTime myDate = DateTime.ParseExact(value!, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch 
            {
                throw new EapException(ErrorCodeEnum.InvalidDatimeFormat, "valid format: yyyy-MM-dd HH:mm:ss");
            }            
        }
    }
}
