using Azure;
using static FIT.MES.Service.CommonEnum;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** 因Unit Test需要Mock Service，故將MES提供的Service再包一層，方便Moq Mock */
    public interface IMesService
    {
        Task<string> CHECK_OP_PASSWORD(string pUserID, string pPassword);
        Task<string> CHECK_SECTION_PERMISSION(string pOpeartorID, string pSectionCode, string pStationCode);
        Task<string> GET_SN_BY_SN_FIXTURE(string pCarrierNo);
        Task<string> UNIT_PROCESS_CHECK(string pSN, string pSectionCode, string pStationCode);
        Task<string> ADD_BOM_DATA(string pLineName, string pSectionCode, string pStationCode, string pSN, string pComponentLot);
        Task<string> UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN);
        Task<string> UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN, string pResult, string pList_of_failing_tests, string pFailure_Message);
        Task<string> GET_SN_BY_SMTSN(string pSMTSN);
        Task<string> BIND_FIXTURE_BY_SN_FIXTURE(string pLineName, string pSectionCode, string pStationCode, string pCarrierNo, string pBindCarrierNo);
        Task<string> GET_SNLIST_BY_FIXTURESN(string pCarrierNo);
        Task<string> SN_LINK_WO(string pLineName, string pSectionCode, string pStationCode, string pWorkOrderNo, string pSN, string pOperator);
        Task<string> GENERATE_SN_BY_WO(string pLineName, string pSectionCode, string pStationCode, string pWorkOrderNo, string pOperator);
        Task<string> GET_INVALIDTIME_BY_SN(string pSn, string pSectionCode, string pStationCode);
        Task<string> UNBIND_SN_FIXTURESN(string pSn);
        Task<string> HOLD_SNLIST_COMMIT(string pSn);
        Task<string> GET_SN_BY_RAWSN(string pSn);
        /**
         * SMT 大小板綁定 
         * pWorkOrderNo 工單
         * pPartentSN 大板子SN
         * pChildSNList 小板子SN
         * pCarrierNo 載具
         * pOperator 是 user
         */
        Task<string> SPLITE_SN_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pWorkOrderNo, string pPartentSN, string pChildSNList, string pCarrierNo, string pOperator);
    }
}
