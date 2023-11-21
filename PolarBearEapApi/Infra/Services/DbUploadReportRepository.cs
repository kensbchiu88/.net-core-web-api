using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Interfaces;

namespace PolarBearEapApi.Infra.Services
{
    public class DbUploadReportRepository : IUploadReportRepository
    {
        private readonly UploadReportDbContext _context;

        public DbUploadReportRepository(UploadReportDbContext context)
        {
            _context = context;
        }
        public async Task<UploadReportEntity> Insert(UploadReportEntity entity)
        {
            _context.UploadInfoEnties.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
