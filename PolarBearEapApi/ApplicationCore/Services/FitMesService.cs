using FIT.MES.GlueRuleLibrary;
using FIT.MES.Service;
using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class FitMesService : IMesService
    {
        private readonly EquipmentService _equipmentService;
        private readonly IStoredProcedureResultRepository _storeProcedureResultRepository;

        public FitMesService(EquipmentService equipmentService, IStoredProcedureResultRepository storeProcedureResultRepository)
        {
            _equipmentService = equipmentService;
            _storeProcedureResultRepository = storeProcedureResultRepository;
        }

        public async Task<string> ADD_BOM_DATA(string pLineName, string pSectionCode, string pStationCode, string pSN, string pComponentLot)
        {
            var result = await Task.Run(() => _equipmentService.ADD_BOM_DATA(pLineName, pSectionCode, pStationCode, pSN, pComponentLot));
            return result;
        }

        public async Task<string> CHECK_OP_PASSWORD(string pUserID, string pPassword)
        {
            var result = await Task.Run(() => _equipmentService.CHECK_OP_PASSWORD(pUserID, pPassword));
            return result;
        }

        public async Task<string> CHECK_SECTION_PERMISSION(string pOpeartorID, string pSectionCode, string pStationCode)
        {
            var result = await Task.Run(() => _equipmentService.CHECK_SECTION_PERMISSION(pOpeartorID, pSectionCode, pStationCode));
            return result;
        }

        public async Task<string> GET_SN_BY_SN_FIXTURE(string pCarrierNo)
        {
            var result = await Task.Run(() => _equipmentService.GET_SN_BY_SN_FIXTURE(pCarrierNo));
            return result;
        }

        public async Task<string> UNIT_PROCESS_CHECK(string pSN, string pSectionCode, string pStationCode)
        {
            var result = await Task.Run(() => _equipmentService.UNIT_PROCESS_CHECK(pSN, pSectionCode, pStationCode));
            return result;
        }

        public async Task<string> UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN)
        {
            var result = await Task.Run(() => _equipmentService.UNIT_PROCESS_COMMIT(pLineName, pSectionCode, pStationCode, pSN));
            return result;
        }

        public async Task<string> UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN, string pResult, string pList_of_failing_tests, string pFailure_Message)
        {
            var result = await Task.Run(() => _equipmentService.UNIT_PROCESS_COMMIT(pLineName, pSectionCode, pStationCode, pSN, pResult, pList_of_failing_tests, pFailure_Message));
            return result;
        }

        public async Task<string> GET_SN_BY_SMTSN(string pSMTSN)
        {
            var result = await Task.Run(() => _equipmentService.GET_SN_BY_SMTSN(pSMTSN));
            return result;
        }

        public async Task<string> BIND_FIXTURE_BY_SN_FIXTURE(string pLineName, string pSectionCode, string pStationCode, string pCarrierNo, string pBindCarrierNo)
        {
            var result = await Task.Run(() => _equipmentService.BIND_FIXTURE_BY_SN_FIXTURE(pLineName, pSectionCode, pStationCode, pCarrierNo, pBindCarrierNo));
            return result;
        }
        public async Task<string> GET_SNLIST_BY_FIXTURESN(string pSectionCode, string pStationCode, string pCarrierNo)
        {
            var result = await Task.Run(() => _equipmentService.GET_SNLIST_BY_FIXTURESN(pSectionCode, pStationCode, pCarrierNo));
            return result;
        }

        public async Task<string> SN_LINK_WO(string pLineName, string pSectionCode, string pStationCode, string pWorkOrderNo, string pSN, string pOperator)
        {
            var result = await Task.Run(() => _equipmentService.SN_LINK_WO(pLineName, pSectionCode, pStationCode, pWorkOrderNo, pSN, pOperator));
            return result;
        }

        public async Task<string> GENERATE_SN_BY_WO(string pLineName, string pSectionCode, string pStationCode, string pWorkOrderNo, string pOperator)
        {
            var result = await Task.Run(() => _equipmentService.GENERATE_SN_BY_WO(pLineName, pSectionCode, pStationCode, pWorkOrderNo, pOperator));
            return result;
        }


        public async Task<string> GET_INVALIDTIME_BY_SN(string pSn, string pSectionCode, string pStationCode)
        {
            FITMesResponse response = new FITMesResponse();
            var mesOperationName = await _storeProcedureResultRepository.GetMesOperation(pSectionCode, pStationCode);
            if (string.IsNullOrEmpty(mesOperationName))
            {
                response.Result = "NG";
                response.Display = "MES station is not defined";
            }
            else
            {
                string mesReturn = await Task.Run(() => GuleRuleUtils.CheckGule(pSn, mesOperationName));
                var mesReturnList = mesReturn.Split(',').ToList();

                
                if (mesReturnList.Count == 0)
                {
                    response.Result = "NG";
                    response.Display = "No Data Found";
                }
                else
                {
                    if ("OK".Equals(mesReturnList[0], StringComparison.OrdinalIgnoreCase))
                    {
                        response.Result = "OK";
                        response.ResultCode = mesReturnList[1];
                    }
                    else
                    {
                        response.Result = "NG";
                        response.Display = mesReturnList[2];
                    }
                }
                
            }

            var result = JsonConvert.SerializeObject(response);

            return result;
        }

        public async Task<string> UNBIND_SN_FIXTURESN(string pSn)
        { 
            var result = await _storeProcedureResultRepository.UnbindSnFixtureSn(pSn);
            return result;
        }

        public async Task<string> GET_SN_BY_RAWSN(string pSn)
        {
            var result = await _storeProcedureResultRepository.GetSnByRawsn(pSn);
            return result;
        }
        
        public async Task<string> HOLD_SNLIST_COMMIT(string sn)
        {
            var result = await _storeProcedureResultRepository.HoldSnlistCommit(sn);
            return result;
        }
        public async Task<string> SPLITE_SN_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pWorkOrderNo, string pPartentSN, string pChildSNList, string pCarrierNo, string pOperator)
        {
            var result = await Task.Run(() => _equipmentService.SPLITE_SN_COMMIT(pLineName, pSectionCode, pStationCode, pWorkOrderNo, pPartentSN, pChildSNList, pCarrierNo, pOperator));
            return result;
        }

        public async Task<string> GET_QTIME_START(string sn, string sectionCode, string stationCode)
        {
            string result = string.Empty;
            FITMesResponse response = new FITMesResponse();
            var mesOperationName = await _storeProcedureResultRepository.GetMesOperation(sectionCode, stationCode);
            if (string.IsNullOrEmpty(mesOperationName))
            {
                response.Result = "NG";
                response.Display = "MES station is not defined";
                result = JsonConvert.SerializeObject(response);
            }
            else
            {
                result = await _storeProcedureResultRepository.GetQtimeStart(sn, mesOperationName);
            }

            return result;
        }

        public async Task<string> GET_BADMARK(string sn)
        {
            var result = await _storeProcedureResultRepository.GetBadmark(sn);
            return result;
        }

        public async Task<string> BCC_SN_CREATE_AUTO(string pWorkOrderNo, int pRequestQty, string pOperator)
        {
            var result = await Task.Run(() => _equipmentService.BCC_SN_Create_Auto(pWorkOrderNo, pRequestQty, pOperator));
            return result;
        }

        public async Task<string> LB_BINGDING_WP_PM(string pWorkOrderNo, string pParentSN, string pChildSNList, string pSectionCode, string pStationCode, string pOperator)
        {
            var result = await Task.Run(() => _equipmentService.LB_BINGDING_WP_PM(pWorkOrderNo, pParentSN, pChildSNList, pSectionCode, pStationCode, pOperator));
            return result;
        }

        public async Task<string> SMT_UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN, string pResult, string pBin, string pBadMark)
        {
            var result = await Task.Run(() => _equipmentService.SMT_UNIT_PROCESS_COMMIT(pLineName, pSectionCode, pStationCode, pSN, pResult, pBin, pBadMark));
            return result;
        }
    }
}
