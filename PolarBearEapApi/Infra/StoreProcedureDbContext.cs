using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.ApplicationCore.Entities;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Xml;
using static FIT.MES.Service.CommonEnum;

namespace PolarBearEapApi.Infra
{
    public class StoreProcedureDbContext : DbContext
    {
        public StoreProcedureDbContext(DbContextOptions<StoreProcedureDbContext> options) : base(options)
        {
        }        
        public DbSet<StoredProcedureResultEntity> StoredProcedureResultEntities { get; set; }
    }
}
