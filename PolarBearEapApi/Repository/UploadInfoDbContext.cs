using Microsoft.EntityFrameworkCore;

namespace PolarBearEapApi.Repository
{
    public class UploadInfoDbContext : DbContext
    {
        public UploadInfoDbContext(DbContextOptions<UploadInfoDbContext> options)
        : base(options)
        {
        }

        public DbSet<UploadInfoEntity> UploadInfoEnties { get; set; }

    }
}
