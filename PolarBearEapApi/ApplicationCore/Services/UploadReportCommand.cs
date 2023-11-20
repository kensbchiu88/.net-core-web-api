using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class UploadReportCommand : IMesCommand
    {
        public string CommandName { get; } = "UPLOAD_REPORT";

        private readonly IUploadReportRepository _uploadReportRepository;
        private readonly ILogger<UploadReportCommand> _logger;

        public UploadReportCommand(IUploadReportRepository uploadReportRepository, ILogger<UploadReportCommand> logger)
        {
            _uploadReportRepository = uploadReportRepository;
            _logger = logger;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input);

            string lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode")!;
            string sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode")!;
            string stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode")!;
            string sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN")!;
            string opRequestInfo = JsonUtil.RemoveSn(JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo")!).Replace(Environment.NewLine, string.Empty);

            try
            {
                //insert into db
                var entity = new UploadReportEntity
                {
                    LineCode = lineCode,
                    SectionCode = sectionCode,
                    StationCode = int.Parse(stationCode),
                    Sn = sn,
                    OpRequestInfo = opRequestInfo,
                    UploadTime = DateTime.Now
                };
                await _uploadReportRepository.Insert(entity);

                return MesCommandResponse.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.ToString()));
                return MesCommandResponse.Fail(ErrorCodeEnum.UploadFail, ex.Message);
            }
        }

        private static void ValidateInput(MesCommandRequest input)
        {
            List<string> requiredFields = new List<string>();

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");
            string? opRequestInfo = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(sn))
                requiredFields.Add("OPRequestInfo.SN");
            if (string.IsNullOrEmpty(opRequestInfo))
                requiredFields.Add("OPRequestInfo");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }
    }
}
