using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IStoredProcedureResultRepository
    {
        Task<string> GetMesOperation(string sectionCode, string stationCode);
        Task<string> UnbindSnFixtureSn(string sn);
        Task<string> GetSnByRawsn(string sn);
    }
}
