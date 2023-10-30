using FIT.MES.Service;
using PolarBearEapApi.ApplicationCore.Interfaces;

namespace PolarBearEapApi.ApplicationCore.Services
{
    public class FitMesService : IMesService
    {
        private readonly EquipmentService _equipmentService;

        public FitMesService(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        async Task<string> IMesService.ADD_BOM_DATA(string pLineName, string pSectionCode, string pStationCode, string pSN, string pComponentLot)
        {
            var result = await Task.Run( () => _equipmentService.ADD_BOM_DATA(pLineName, pSectionCode, pStationCode, pSN, pComponentLot));
            return result;
        }

        async Task<string> IMesService.CHECK_OP_PASSWORD(string pUserID, string pPassword)
        {
            var result = await Task.Run(() => _equipmentService.CHECK_OP_PASSWORD(pUserID, pPassword));
            return result;
        }

        async Task<string> IMesService.CHECK_SECTION_PERMISSION(string pOpeartorID, string pSectionCode, string pStationCode)
        {
            var result = await Task.Run(() => _equipmentService.CHECK_SECTION_PERMISSION(pOpeartorID, pSectionCode, pStationCode));
            return result;
        }

        async Task<string> IMesService.GET_SN_BY_SN_FIXTURE(string pCarrierNo)
        {
            var result = await Task.Run(() => _equipmentService.GET_SN_BY_SN_FIXTURE(pCarrierNo));
            return result;
        }

        async Task<string> IMesService.UNIT_PROCESS_CHECK(string pSN, string pSectionCode, string pStationCode)
        {
            var result = await Task.Run(() => _equipmentService.UNIT_PROCESS_CHECK(pSN, pSectionCode, pStationCode));
            return result;
        }

        async Task<string> IMesService.UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN)
        {
            var result = await Task.Run(() => _equipmentService.UNIT_PROCESS_COMMIT(pLineName, pSectionCode, pStationCode, pSN));
            return result;
        }

        async Task<string> IMesService.UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN, string pResult, string pList_of_failing_tests, string pFailure_Message)
        {
            var result = await Task.Run(() => _equipmentService.UNIT_PROCESS_COMMIT(pLineName, pSectionCode, pStationCode, pSN, pResult, pList_of_failing_tests, pFailure_Message));
            return result;
        }

        async Task<string> IMesService.GET_SN_BY_SMTSN(string pSMTSN)
        {
            var result = await Task.Run(() => _equipmentService.GET_SN_BY_SMTSN(pSMTSN));
            return result;
        }
    }
}
