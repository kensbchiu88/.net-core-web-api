using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.PublicApi.Models;
using PolarBearEapApi.ApplicationCore.Entities;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class UploadInfosCommand : IMesCommand
    {
        public string CommandName { get; } = "UPLOAD_INFOS";

        private readonly IUploadInfoRepository _uploadInfoService;
        private readonly ILogger<UploadInfosCommand> _logger;

        public UploadInfosCommand(IUploadInfoRepository uploadInfoService, ILogger<UploadInfosCommand> logger)
        {
            _uploadInfoService = uploadInfoService;
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
                var entity = new UploadInfoEntity
                {
                    LineCode = lineCode,
                    SectionCode = sectionCode,
                    StationCode = int.Parse(stationCode),
                    Sn = sn,
                    OpRequestInfo = opRequestInfo,
                    UploadTime = DateTime.Now
                };
                await _uploadInfoService.Insert(entity);

                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.Message));
                return Fail(ErrorCodeEnum.UploadFail);
            }
        }

        private MesCommandResponse Success()
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"OK\"}";
            return response;
        }

        private MesCommandResponse Fail(ErrorCodeEnum errorCodeEnum)
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"NG\"}";
            response.ErrorMessage = errorCodeEnum.ToString();
            return response;
        }

        private MesCommandResponse FailWithErrorMessage(string message)
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"NG\"}";
            response.ErrorMessage = message;
            return response;
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
