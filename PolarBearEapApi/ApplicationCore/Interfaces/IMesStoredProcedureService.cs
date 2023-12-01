using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** 呼叫 MES Stored Procedure 的介面 */
    public interface IMesStoredProcedureService
    {
        Task<string> GetMesOperation(string sectionCode, string stationCode);
        Task<string> UnbindSnFixtureSn(string sn);
        Task<string> HoldSnlistCommit(string sn);
        Task<string> GetSnByRawsn(string sn);
        Task<string> GetQtimeStart(string sn, string operationName);
        Task<string> GetBadmark(string sn);
        Task<string> CheckUcClear(string operationName, string carrierNo);
    }
}
