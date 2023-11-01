using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class GetInputDataCommand : IMesCommand
    {
        private readonly ILogger<GetInputDataCommand> _logger;
        private readonly IUploadInfoRepository _uploadInfoService;

        string IMesCommand.CommandName { get; } = "GET_INPUT_DATA";

        public GetInputDataCommand(ILogger<GetInputDataCommand> logger, IUploadInfoRepository uploadInfoService)
        {
            _logger = logger;
            _uploadInfoService = uploadInfoService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            int stationCode = int.Parse(JsonUtil.GetParameter(input.SerializeData, "StationCode") ?? "0");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");

            try
            {
                var uploadInfo = await _uploadInfoService.GetOne(lineCode, sectionCode, stationCode, sn);
                if (uploadInfo != null)
                {
                    return Success(uploadInfo.OpRequestInfo.Replace("\\", "\\\\\\"), uploadInfo.UploadTime);
                }
                else
                {
                    return NoDataFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, e.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, e.Message));
                return Fail(ErrorCodeEnum.QueryDbError);
            }
        }

        private MesCommandResponse Success(string returnString)
        {
            //return "{\"Data\":\"{\\\"ERROR_CODE\\\":\\\"NA\\\",\\\"IS_SHA1\\\":\\\"caf0c6308ec37cdd1026e8ef4d185d0658154f84\\\",\\\"VL3_SHA1\\\":\\\"042b711bfc8385010d4077ea2a74606b697dfcaf\\\",\\\"FIXTURE-UC110\\\":\\\"Olyto202205141758\\\",\\\"VL2_SHA1\\\":\\\"1515a96fa39e1084e5776bda99bf552807249a77\\\",\\\"CD_SHA1\\\":\\\"7b8bab4ecb7d3990fa03f0b3779545642ce340e6\\\",\\\"VS_SHA1\\\":\\\"18144a38004ad8da345ff9cfc3ad80c0dc4c5dc4\\\",\\\"MS_SHA1\\\":\\\"9eee1c4a011fcc49762401a703fefc4332649dab\\\",\\\"VL1_SHA1\\\":\\\"bf22cca14b892a1253407d249f23624d769d426d\\\"}";
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Data\":\"" + returnString.Replace("\"", "\\\"") + "\"}";
            return response;
        }

        private MesCommandResponse Success(string returnString, DateTime inputDate)
        {
            //return "{\"Data\":\"{\\\"ERROR_CODE\\\":\\\"NA\\\",\\\"IS_SHA1\\\":\\\"caf0c6308ec37cdd1026e8ef4d185d0658154f84\\\",\\\"VL3_SHA1\\\":\\\"042b711bfc8385010d4077ea2a74606b697dfcaf\\\",\\\"FIXTURE-UC110\\\":\\\"Olyto202205141758\\\",\\\"VL2_SHA1\\\":\\\"1515a96fa39e1084e5776bda99bf552807249a77\\\",\\\"CD_SHA1\\\":\\\"7b8bab4ecb7d3990fa03f0b3779545642ce340e6\\\",\\\"VS_SHA1\\\":\\\"18144a38004ad8da345ff9cfc3ad80c0dc4c5dc4\\\",\\\"MS_SHA1\\\":\\\"9eee1c4a011fcc49762401a703fefc4332649dab\\\",\\\"VL1_SHA1\\\":\\\"bf22cca14b892a1253407d249f23624d769d426d\\\"}";
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"InputDate\": \"" + inputDate.ToString("yyyy-MM-dd HH:mm:ss") + "\" ,\"Data\":\"" + returnString.Replace("\"", "\\\"") + "\"}";
            return response;
        }

        /** Fail. return {\"Data\":\"{}\"} */
        private MesCommandResponse Fail(ErrorCodeEnum errorCodeEnum)
        {
            MesCommandResponse response = new MesCommandResponse();
            response.ErrorMessage = errorCodeEnum.ToString();
            response.OpResponseInfo = "{\"Data\":\"{}\"}";
            return response;
        }

        private MesCommandResponse NoDataFound()
        {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Data\":\"{}\"}";
            response.ErrorMessage = ErrorCodeEnum.NoDataFound.ToString();
            return response;
        }
    }
}
