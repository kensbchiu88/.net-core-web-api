﻿using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class SmtUnitProcessCommitCommand : IMesCommand
    {
        public string CommandName { get; } = "SMT_UNIT_PROCESS_COMMIT";           

        private readonly IMesService _equipmentService;
        private readonly IEquipmentTemporaryDataRepository _repository;
        private readonly ILogger<SmtUnitProcessCommitCommand> _logger;

        public SmtUnitProcessCommitCommand(IMesService equipmentService, IEquipmentTemporaryDataRepository repository, ILogger<SmtUnitProcessCommitCommand> logger)
        {
            _equipmentService = equipmentService;
            _repository = repository;
            _logger = logger;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            SerializeDataModel inputModel = JsonConvert.DeserializeObject<SerializeDataModel>(input.SerializeData);
            ValidateInput(inputModel);

            string mesReturn = string.Empty;
            bool isSave2DbSuccess = false;
            try
            {
                mesReturn = await _equipmentService.SMT_UNIT_PROCESS_COMMIT(inputModel.LineCode!, inputModel.SectionCode!, inputModel.SectionCode!.ToString(), inputModel.OPRequestInfo!.Sn!, inputModel.OPRequestInfo.Result!, inputModel.OPRequestInfo.BinData, inputModel.OPRequestInfo.BadMark);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }

            //if (FITMesResponse.IsResultOk(mesReturn))
            {
                try
                {
                    //insert into db
                    var entity = new EquipmentTemporaryDataEntity
                    {
                        LineCode = inputModel.LineCode!,
                        SectionCode = inputModel.SectionCode!,
                        StationCode = inputModel.StationCode ?? 0,
                        Sn = inputModel.OPRequestInfo.Sn!,
                        DataKey = CommandName,
                        DataValue = GetSaveContent(inputModel.OPRequestInfo),
                        CreateTime = DateTime.UtcNow
                    };
                    await _repository.Insert(entity);

                    isSave2DbSuccess = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.ToString()));
                    isSave2DbSuccess = false;
                }
            }

            return GetResponse(mesReturn, isSave2DbSuccess);
        }

        private static void ValidateInput(SerializeDataModel inputModel)
        {
            List<string> requiredFields = new List<string>();

            if (string.IsNullOrEmpty(inputModel.LineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(inputModel.SectionCode))
                requiredFields.Add("SectionCode");
            if (inputModel.StationCode == null)
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.Sn))
                requiredFields.Add("OPRequestInfo.SN");
            if (string.IsNullOrEmpty(inputModel.OPRequestInfo.Result))
                requiredFields.Add("OPRequestInfo.Result");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }

        private string GetSaveContent(OpRequestInfoModel opRequestInfoModel)
        {
            var newModel = new OpRequestInfoModel
            {
                Result = opRequestInfoModel.Result,
                BinData = opRequestInfoModel.BinData,
                BadMark = opRequestInfoModel.BadMark,
                ListOfFailingTests = opRequestInfoModel.ListOfFailingTests,
                FailureMessage = opRequestInfoModel.FailureMessage
            };

            return JsonConvert.SerializeObject(newModel,Formatting.None,new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        private static MesCommandResponse GetResponse(string mesReturn, bool isSave2DbSuccess)
        {
            var response = new MesCommandResponse();
            var fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturn);

            if (fitMesResponse != null)
            {
                if (fitMesResponse.IsResultOk() && isSave2DbSuccess)
                {
                    response.OpResponseInfo = "{\"Result\":\"OK\"}";
                }
                else 
                {
                    response.OpResponseInfo = "{\"Result\":\"NG\"}";

                    if (!fitMesResponse.IsResultOk())
                    {
                        response.ErrorMessage = fitMesResponse.Display;
                    }
                    if (!isSave2DbSuccess)
                    {
                        response.ErrorMessage = ErrorCodeEnum.UploadFail.ToString();
                    }
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
            public string? Sn { get; set; }
            public string? Result { get; set; }
            public string? BinData { get; set; }
            public string? BadMark { get; set; }

            [JsonProperty("List_Of_Failing_Tests")]
            public string? ListOfFailingTests;

            [JsonProperty("Failure_Message")]
            public string? FailureMessage;
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
