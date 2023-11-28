using Microsoft.Data.SqlClient;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.PublicApi.Models;
using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.ApplicationCore.Interfaces;
using Newtonsoft.Json;

namespace PolarBearEapApi.Infra.Services
{
    public class SqlServerStoredProcedureResultRepository : IStoredProcedureResultRepository
    {
        private readonly StoreProcedureDbContext _context;

        public SqlServerStoredProcedureResultRepository(StoreProcedureDbContext context)
        {
            _context =  context;
        }

        /**
         * 由section code, station code 對應出 mes 的 operation name (mes的站名) 
         * 
         * return: Mes Operation Name
         */
        public async Task<string> GetMesOperation(string sectionCode, string stationCode)
        {
            var result = string.Empty;
            var sectionCodeParam = new SqlParameter("@SECTIONCODE", sectionCode);
            var stationCodeParam = new SqlParameter("@STATIONCODE", stationCode);

            List<StoredProcedureResultEntity> porcedureResult;

            porcedureResult = await _context.StoredProcedureResultEntities.FromSql($"dbo.sp_AUTO_GETOPERATION_BYSECTION {sectionCode}, {stationCode}").ToListAsync();

            if (porcedureResult.Any())
            { 
                var results = porcedureResult.First().Result.Split(',').ToList();
                if (results.Count == 3 && "OK".Equals(results[0], StringComparison.OrdinalIgnoreCase)) 
                { 
                    result = results[2];
                }
            }

            return result;
        }

        public async Task<string> UnbindSnFixtureSn(string sn)
        {
            var snParam = new SqlParameter("@SN", sn);

            List<StoredProcedureResultEntity> porcedureResult;

            porcedureResult = await _context.StoredProcedureResultEntities.FromSql($"dbo.sp_AUTO_UNBIND_SN_FIXTURESN {snParam}").ToListAsync();

            return ConvertStoredProcedureResultToFitMesResponseString(porcedureResult);
        }

        public async Task<string> HoldSnlistCommit(string sn)
        {
            var snParam = new SqlParameter("@SN", sn);

            List<StoredProcedureResultEntity> porcedureResult;

            porcedureResult = await _context.StoredProcedureResultEntities.FromSql($"dbo.sp_AUTO_HOLD_SNLIST_COMMIT {snParam}").ToListAsync();

            return ConvertStoredProcedureResultToFitMesResponseString(porcedureResult);
        }


        public async Task<string> GetSnByRawsn(string sn)
        {
            var snParam = new SqlParameter("@SN", sn);

            List<StoredProcedureResultEntity> porcedureResult;

            porcedureResult = await _context.StoredProcedureResultEntities.FromSql($"sp_AUTO_GET_SN_BY_RAWSN {snParam}").ToListAsync();

            return ConvertStoredProcedureResultToFitMesResponseString(porcedureResult);
        }

        public async Task<string> GetQtimeStart(string sn, string operationName)
        {
            var snParam = new SqlParameter("@SN", sn);
            var operationNameParam = new SqlParameter("@OPERATIONNAME", operationName);

            List<StoredProcedureResultEntity> porcedureResult;

            porcedureResult = await _context.StoredProcedureResultEntities.FromSql($"sp_AUTO_GET_QTIME_START {snParam}, {operationNameParam}").ToListAsync();

            return ConvertStoredProcedureResultToFitMesResponseString(porcedureResult);
        }

        public async Task<string> GetBadmark(string sn)
        {
            var snParam = new SqlParameter("@SN", sn);

            List<StoredProcedureResultEntity> porcedureResult;

            porcedureResult = await _context.StoredProcedureResultEntities.FromSql($"sp_AUTO_GET_BADMARK {snParam}").ToListAsync();

            return ConvertStoredProcedureResultToFitMesResponseString(porcedureResult);
        }

        public async Task<string> CheckUcClear(string operationName, string carrierNo)
        {
            var operationNameParam = new SqlParameter("@operation", operationName);
            var carrierParam = new SqlParameter("@carrierno", carrierNo);

            List<StoredProcedureResultEntity> porcedureResult;

            porcedureResult = await _context.StoredProcedureResultEntities.FromSql($"sp_AUTO_CHECK_UC_CLEAR {operationNameParam}, {carrierParam}").ToListAsync();

            return ConvertStoredProcedureResultToFitMesResponseString(porcedureResult);
        }

        private string ConvertStoredProcedureResultToFitMesResponseString(List<StoredProcedureResultEntity> porcedureResult)
        {
            FITMesResponse fitResponse;

            if (!porcedureResult.Any())
            {
                fitResponse = new FITMesResponse
                {
                    Result = "NG",
                    Display = "No Data Return"
                };
            }
            else
            {
                fitResponse = FITMesResponse.ConvertFromStoredProcedureResult(porcedureResult.First().Result);
            }

            return JsonConvert.SerializeObject(fitResponse);
        }

    }
}
