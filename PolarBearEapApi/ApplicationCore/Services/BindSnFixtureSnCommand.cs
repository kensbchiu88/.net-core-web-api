﻿using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class BindSnFixtureSnCommand : IMesCommand
    {
        public string CommandName { get; } = "BIND_SN_FIXTURESN";

        private readonly IMesService _equipmentService;

        public BindSnFixtureSnCommand(IMesService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        public async Task<MesCommandResponse> Execute(MesCommandRequest input)
        {
            string? lineCode = JsonUtil.GetParameter(input.SerializeData, "LineCode");
            string? sectionCode = JsonUtil.GetParameter(input.SerializeData, "SectionCode");
            string? stationCode = JsonUtil.GetParameter(input.SerializeData, "StationCode");
            string? sn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.SN"); 
            string? fixtureSn = JsonUtil.GetParameter(input.SerializeData, "OPRequestInfo.FIXTURE_SN");

            try
            {
                string mesReturn = await _equipmentService.BIND_FIXTURE_BY_SN_FIXTURE(lineCode, sectionCode, stationCode, sn, fixtureSn);
                return new MesCommandResponse(mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
        }
    }
}
