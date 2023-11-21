﻿using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;
using static Azure.Core.HttpHeader;

namespace PolarBearEapApi.PublicApi.Soap
{
    public class WebService : IWebService
    {
        private readonly IMesService _equipmentService;
        private readonly ILogger<WebService> _logger;

        public WebService(IMesService equipmentService, ILogger<WebService> logger)
        {
            _equipmentService = equipmentService;
            _logger = logger;
        }

        public async Task<bool> UserLogin(string user, string password)
        {
            bool result = false;
            string mesReturn;
            try
            {                
                mesReturn = await _equipmentService.CHECK_OP_PASSWORD(user, password);
                _logger.LogInformation("UserLogin user:{user}, password:{password}, return:{mesReturn}", user, password, mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
            //驗證MES結果
            if (FITMesResponse.IsResultOk(mesReturn))
            {
                result = true;
            }

            return result;
        }

        public List<List<string>> GetLineList()
        {
            var result = new List<List<string>>();
            var line1 = new List<string>(){ "LINE_CODE1", "LINE_NAME1", "LINE_TYPE1", "LINE_DESC1", "CLASS_CODE1"};
            var line2 = new List<string>(){ "LINE_CODE2", "LINE_NAME2", "LINE_TYPE2", "LINE_DESC2", "CLASS_CODE2"};

            result.Add(line1);
            result.Add(line2);

            return result;
        }

        public async Task<string> GetStationByLineSection(string lineCode, string sectionCode = "0")
        { 
            var result = new List<string>() { "CLASS_CODE", "LINE_CODE", "LINE_NAME", "SECTION_CODE", "SECTION_DESC", "STATION_CODE", "STATION_NAME" };
            return String.Join(";", result.ToArray());
        }

        public async Task<bool> CheckDevRoute(string sn, string sectionCode, string sectionDesc, string stationCode, string StationDesc, string lineCode, string tester)
        {
            bool result = false;
            string mesReturn;
            try
            {
                mesReturn = await _equipmentService.UNIT_PROCESS_CHECK(sn!, sectionCode!, stationCode!);
                _logger.LogInformation("CheckDevRoute sn:{sn}, sectionCode:{sectionCode}, stationCode:{stationCode}, return:{mesReturn}", sn, sectionCode, stationCode, mesReturn);
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
            //驗證MES結果
            if (FITMesResponse.IsResultOk(mesReturn))
            {
                result = true;
            }

            return result;
        }

        public async Task<bool> CommitDevData(string sn, string sectionCode, string sectionDesc, string stationCode, string stationDesc, string lineCode, string tester, string test_time, int testResult, string testData)
        {
            bool result = false;
            string mesReturn;
            try
            {
                if (testResult == 1)
                {
                    mesReturn = await _equipmentService.UNIT_PROCESS_COMMIT(lineCode, sectionCode, stationCode, sn);
                    _logger.LogInformation("CommitDevData lineCode:{lineCode}, sectionCode:{sectionCode}, stationCode:{stationCode}, sn:{sn}, return:{mesReturn}", lineCode, sectionCode, stationCode, sn, mesReturn);
                }
                else
                {
                    mesReturn = await _equipmentService.UNIT_PROCESS_COMMIT(lineCode!, sectionCode!, stationCode!, sn!, "FAIL", "", "");
                    _logger.LogInformation("CommitDevData lineCode:{lineCode}, sectionCode:{sectionCode}, stationCode:{stationCode}, sn:{sn}, result:FAIL, list:{list}, message:{message} return:{mesReturn}", lineCode, sectionCode, stationCode, sn, " ", " ", mesReturn);
                }
            }
            catch (Exception ex)
            {
                throw new EapException(ErrorCodeEnum.CallMesServiceException, ex);
            }
            //驗證MES結果
            if (FITMesResponse.IsResultOk(mesReturn))
            {
                result = true;
            }

            return result;
        }
    }
}
