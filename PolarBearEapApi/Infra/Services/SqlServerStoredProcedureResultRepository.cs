﻿using Microsoft.Data.SqlClient;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.PublicApi.Models;
using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using static FIT.MES.Service.CommonEnum;
using Azure;
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

        public async Task<string> GetSnByRawsn(string sn)
        {
            var snParam = new SqlParameter("@SN", sn);

            List<StoredProcedureResultEntity> porcedureResult;

            porcedureResult = await _context.StoredProcedureResultEntities.FromSql($"sp_AUTO_GET_SN_BY_RAWSN {snParam}").ToListAsync();

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
