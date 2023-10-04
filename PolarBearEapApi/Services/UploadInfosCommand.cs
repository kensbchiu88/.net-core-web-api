using PolarBearEapApi.Commons;
using PolarBearEapApi.Repository;
using System.Net.Http.Headers;
using PolarBearEapApi.Models;
using PolarBearEapApi.Commons.Exceptions;

namespace PolarBearEapApi.Services
{
    public class UploadInfosCommand : IMesCommand
    {
        string IMesCommand.CommandName { get; } = "UPLOAD_INFOS";

        private readonly UploadInfoDbContext _uploadInfoDbContext;
        private readonly ILogger<UploadInfosCommand> _logger;

        public UploadInfosCommand(UploadInfoDbContext uploadInfoDbContext, ILogger<UploadInfosCommand> logger)
        {
            _uploadInfoDbContext = uploadInfoDbContext;
            _logger = logger;
        }
        MesCommandResponse IMesCommand.Execute(MesCommandRequest input)
        {
            ValidateInput(input);

            try {
                string lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode")!;
                string sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode")!;
                string stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode")!;
                string sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN")!;
                string opRequestInfo = JsonUtil.RemoveSn(JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo")!).Replace(System.Environment.NewLine, string.Empty);

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
                _uploadInfoDbContext.UploadInfoEnties.Add(entity);
                _uploadInfoDbContext.SaveChanges();
                
                return Success();
            } catch (Exception ex) {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.Message));
                return Fail();
            }
        }

        private MesCommandResponse Success() {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"OK\"}";
            return response;
        }

        private MesCommandResponse Fail()
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Result\":\"NG\"}";
            response.ErrorMessage = ErrorCodeEnum.UploadFail.ToString();
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
                throw new JsonFieldRequireException("Json Fields Required: " + string.Join(",", requiredFields));
        }
    }
}
