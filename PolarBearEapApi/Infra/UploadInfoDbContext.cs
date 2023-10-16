using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.ApplicationCore.Entities;

namespace PolarBearEapApi.Infra
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
