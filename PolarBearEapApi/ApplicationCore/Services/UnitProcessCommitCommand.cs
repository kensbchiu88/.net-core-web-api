﻿using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** UNIT_PROCESS_COMMIT 依據SN，提交過站資料 */
    public class UnitProcessCommitCommand : IMesCommand
    {
        public string CommandName { get; } = "UNIT_PROCESS_COMMIT";

        private readonly ILogger<UnitProcessCommitCommand> _logger;
        private readonly IMesService _equipmentService;

        const string _TEST_FAIL = "FAIL";

        public UnitProcessCommitCommand(ILogger<UnitProcessCommitCommand> logger, IMesService equipmentService)
        {
            _logger = logger;
            _equipmentService = equipmentService;
        }
        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            ValidateInput(input.SerializeData);

            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN");
            string? result = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.result");

            try
            {
                MesCommandResponse response;
                if (!string.IsNullOrEmpty(result))
                {
                    string? errorcode = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.list_of_failing_tests");
                    string? errorMessage = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.failure_message");
                    string mesReturn = await _equipmentService.UNIT_PROCESS_COMMIT(lineCode!, sectionCode!, stationCode!, sn!, _TEST_FAIL, errorcode, errorMessage);
                    response = new MesCommandResponse(mesReturn);
                }
                else
                {
                    string mesReturn = await _equipmentService.UNIT_PROCESS_COMMIT(lineCode!, sectionCode!, stationCode!, sn!);
                    response = new MesCommandResponse(mesReturn);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(input.SerializeData, ex.Message));
                return MesCommandResponse.Fail(ErrorCodeEnum.CallMesServiceException);
            }
        }

        private static void ValidateInput(string serializedData)
        {

            List<string> requiredFields = new List<string>();

            string? lineCode = JsonUtil.GetParameter(serializedData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(serializedData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(serializedData, "StationCode");
            string? sn = JsonUtil.GetCaseSensitiveParameter(serializedData, "OPRequestInfo.SN");

            if (string.IsNullOrEmpty(lineCode))
                requiredFields.Add("LineCode");
            if (string.IsNullOrEmpty(sectionCode))
                requiredFields.Add("SectionCode");
            if (string.IsNullOrEmpty(stationCode))
                requiredFields.Add("StationCode");
            if (string.IsNullOrEmpty(sn))
                requiredFields.Add("OPRequestInfo.SN");

            if (requiredFields.Count > 0)
                throw new EapException(ErrorCodeEnum.JsonFieldRequire, "Json Fields Required: " + string.Join(",", requiredFields));
        }
    }
}
