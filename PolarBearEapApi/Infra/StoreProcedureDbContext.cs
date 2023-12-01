using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.Infra.Entities;

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
