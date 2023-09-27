﻿using PolarBearEapApi.Commons;
using PolarBearEapApi.Repository;
using System.Diagnostics;
using static PolarBearEapApi.Services.IMesCommand;
using PolarBearEapApi.Models;

namespace PolarBearEapApi.Services
{
    public class GetInputDataCommand : IMesCommand
    {
        private readonly ILogger<GetInputDataCommand> _logger;
        private readonly UploadInfoDbContext _uploadInfoDbContext;

        string IMesCommand.CommandName { get; } = "GET_INPUT_DATA";
        public GetInputDataCommand(ILogger<GetInputDataCommand> logger, UploadInfoDbContext uploadInfoDbContext) { 
            _logger = logger;
            _uploadInfoDbContext = uploadInfoDbContext;
        }


        MesCommandResponse IMesCommand.Execute(string serializedData)
        {
            string lineCode = "";
            string sectionCode = "";
            int stationCode = 0;
            string sn = "";
            try {
                lineCode = JsonUtil.GetParameter(serializedData, "LineCode").ToUpper();
                sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode").ToUpper();
                stationCode = int.Parse(JsonUtil.GetParameter(serializedData, "StationCode"));
                sn = JsonUtil.GetParameter(serializedData, "OPRequestInfo.SN").ToUpper();
            } catch (Exception ex) {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(serializedData, ex.Message));
                return Fail(ErrorCodeEnum.ParseJsonError);
            }

            try
            {
                //using (var db = new UploadInfoDbContext())
                {
                    var uploadInfos = _uploadInfoDbContext.UploadInfoEnties
                        .Where(e => e.LineCode.ToUpper().Equals(lineCode) && e.SectionCode.ToUpper().Equals(sectionCode) && e.StationCode == stationCode && e.Sn.ToUpper().Equals(sn));

                    if (uploadInfos.Count() > 0)
                    {
                        var uploadInfo = uploadInfos.OrderByDescending(e => e.UploadTime).First();
                        
                        Debug.WriteLine("UploadInfo:" + uploadInfo.OpRequestInfo.Replace("\\", "\\\\\\"));
 
                        //return Success(uploadInfo.OpRequestInfo.Replace("\\", "\\\\\\"));
                        return Success(uploadInfo.OpRequestInfo.Replace("\\", "\\\\\\"), uploadInfo.UploadTime);

                    }
                    else {
                        return NoDataFound(); 
                    }                
                }
            }
            catch (Exception e) {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(serializedData, e.Message));
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
        private MesCommandResponse Fail(ErrorCodeEnum errorCodeEnum) {
            MesCommandResponse response = new MesCommandResponse();
            response.ErrorMessage = errorCodeEnum.ToString();
            response.OpResponseInfo = "{\"Data\":\"{}\"}";
            return response;
        }

        private MesCommandResponse NoDataFound() {
            MesCommandResponse response = new MesCommandResponse();
            response.OpResponseInfo = "{\"Data\":\"{}\"}";
            response.ErrorMessage = ErrorCodeEnum.NoDataFound.ToString();
            return response;
        }
    }
}
