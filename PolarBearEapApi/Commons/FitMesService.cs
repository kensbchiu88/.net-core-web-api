using FIT.MES.Service;

namespace PolarBearEapApi.Commons
{
    public class FitMesService : IMesService
    {
        private readonly EquipmentService _equipmentService;

        public FitMesService(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        string IMesService.ADD_BOM_DATA(string pLineName, string pSectionCode, string pStationCode, string pSN, string pComponentLot)
        {
            return _equipmentService.ADD_BOM_DATA(pLineName, pSectionCode, pStationCode, pSN, pComponentLot);
        }

        string IMesService.CHECK_OP_PASSWORD(string pUserID, string pPassword)
        {
            return _equipmentService.CHECK_OP_PASSWORD(pUserID, pPassword);
        }

        string IMesService.CHECK_SECTION_PERMISSION(string pOpeartorID, string pSectionCode, string pStationCode)
        {
            return _equipmentService.CHECK_SECTION_PERMISSION(pOpeartorID, pSectionCode, pStationCode);
        }

        string IMesService.GET_SN_BY_SN_FIXTURE(string pCarrierNo)
        {
            return _equipmentService.GET_SN_BY_SN_FIXTURE(pCarrierNo);
        }

        string IMesService.UNIT_PROCESS_CHECK(string pSN, string pSectionCode, string pStationCode)
        {
            return _equipmentService.UNIT_PROCESS_CHECK(pSN, pSectionCode, pStationCode);
        }

        string IMesService.UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN)
        {
            return _equipmentService.UNIT_PROCESS_COMMIT(pLineName, pSectionCode, pStationCode, pSN);
        }

        string IMesService.UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN, string pResult, string pList_of_failing_tests, string pFailure_Message)
        {
            return _equipmentService.UNIT_PROCESS_COMMIT(pLineName, pSectionCode, pStationCode, pSN, pResult, pList_of_failing_tests, pFailure_Message);
        }
    }
}
