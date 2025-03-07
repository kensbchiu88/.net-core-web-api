﻿using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** CHECK_UC_CLEAR 檢查載具是否需要清洗 */
    public class CheckUcClearCommand : IMesCommand
    {
        public string CommandName { get; } = "CHECK_UC_CLEAR";

        private readonly IMesService _equipmentService;

        public CheckUcClearCommand (IMesService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            SerializeDataModel inputModel = JsonConvert.DeserializeObject<SerializeDataModel>(input.SerializeData);
            ValidateInput(inputModel);

            try
            {
                string mesReturn = await _equipmentService.CHECK_UC_CLEAR(inputModel.SectionCode!, inputModel.StationCode.ToString()!, inputModel.OPRequestInfo.FixtureSn!);
                return GetResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
        }

        private static void ValidateInput(SerializeDataModel inputModel)
        {
            List<string> requiredFields = new List<string>();

            if (string.IsNullOrEmpty(inputModel.SectionCode))
                requiredFields.Add("SectionCode");
            if (inputModel.StationCode == null)
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.FixtureSn))
                requiredFields.Add("OPRequestInfo.FIXTURE_SN");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }

        private static MesCommandResponse GetResponse(string mesReturn)
        {
            var response = new MesCommandResponse();
            var fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturn);
            if (fitMesResponse != null)
            {
                if (fitMesResponse.IsResultOk())
                {
                    var result = string.Empty;
                    var usageCount = string.Empty;

                    if(!string.IsNullOrEmpty(fitMesResponse.ResultCode))
                    {
                        var results = fitMesResponse.ResultCode.Split(':');
                        if(results.Length == 2) 
                        {
                            result = results[0];
                            usageCount = results[1];
                        }
                        else
                        {
                            result = fitMesResponse.ResultCode;
                        }
                    }
                    
                    response.OpResponseInfo = "{\"Result\":\"" + result + "\", \"UC\":\"" + usageCount + "\"}"; ;
                }
                else
                {
                    response.OpResponseInfo = "{\"Result\":\"NG\"}";
                    response.ErrorMessage = fitMesResponse.Display;
                }
            }
            else
            {
                response.OpResponseInfo = "{\"Result\":\"NG\"}";
                response.ErrorMessage = ErrorCodeEnum.NoMesReturn.ToString();
            }

            return response;
        }

        private class OpRequestInfoModel
        {
            [JsonProperty("FIXTURE_SN")]
            public string? FixtureSn { get; set; }
        }

        private class SerializeDataModel
        {
            public string? LineCode { get; set; }
            public string? SectionCode { get; set; }
            public int? StationCode { get; set; }
            [Required]
            public OpRequestInfoModel? OPRequestInfo { get; set; }
        }
    }
}
