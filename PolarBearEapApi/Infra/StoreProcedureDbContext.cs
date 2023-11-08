using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.ApplicationCore.Entities;
using System.Xml;

namespace PolarBearEapApi.Infra
{
    public class StoreProcedureDbContext : DbContext
    {
        public StoreProcedureDbContext(DbContextOptions<StoreProcedureDbContext> options) : base(options)
        {
        }        
        public DbSet<string> MyStoredProcedureResults { get; set; }

        public List<string> GetMyEntitiesFromStoredProcedure(string parameter1, string parameter2)
        {
            return this.MyStoredProcedureResults.FromSqlRaw("EXECUTE dbo.sp_AUTO_GETOPERATION_BYSECTION @SECTIONCODE, @STATIONCODE", parameter1, parameter2).ToList();
        }
    }
}
