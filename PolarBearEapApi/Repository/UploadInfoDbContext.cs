using Microsoft.EntityFrameworkCore;

namespace PolarBearEapApi.Repository
{
    public class UploadInfoDbContext : DbContext
    {
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=10.35.7.220;Database=FIT_EquipmentData;User Id=eqpmuser;Password=foxconn;TrustServerCertificate=true;");
            optionsBuilder.UseSqlServer("Server=10.222.48.154;Database=FIT_EngineeringData;User Id=eqpmuser;Password=foxconn;TrustServerCertificate=true;");
        }
        */

        public UploadInfoDbContext(DbContextOptions<UploadInfoDbContext> options)
        : base(options)
        {
        }

        public DbSet<UploadInfoEntity> UploadInfoEnties { get; set; }

    }
}
