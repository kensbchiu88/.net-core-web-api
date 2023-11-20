using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.ApplicationCore.Entities;

namespace PolarBearEapApi.Infra
{
    public class UploadReportDbContext : DbContext
    {
        public UploadReportDbContext(DbContextOptions<UploadReportDbContext> options) : base(options)
        {
        }

        public DbSet<UploadReportEntity> UploadInfoEnties { get; set; }
    
    }
}
