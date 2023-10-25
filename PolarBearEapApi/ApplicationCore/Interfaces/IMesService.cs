namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** 因Unit Test需要Mock Service，故將MES提供的Service再包一層，方便Moq Mock */
    public interface IMesService
    {
        string CHECK_OP_PASSWORD(string pUserID, string pPassword);
        string CHECK_SECTION_PERMISSION(string pOpeartorID, string pSectionCode, string pStationCode);
        string GET_SN_BY_SN_FIXTURE(string pCarrierNo);
        string UNIT_PROCESS_CHECK(string pSN, string pSectionCode, string pStationCode);
        string ADD_BOM_DATA(string pLineName, string pSectionCode, string pStationCode, string pSN, string pComponentLot);
        string UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN);
        string UNIT_PROCESS_COMMIT(string pLineName, string pSectionCode, string pStationCode, string pSN, string pResult, string pList_of_failing_tests, string pFailure_Message);
        string GET_SN_BY_SMTSN(string pSMTSN);
    }
}
