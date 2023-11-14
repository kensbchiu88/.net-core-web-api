using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.Infra;

namespace PolarBearEapApi.Infra.Services
{
    public class DbUploadInfoRepository : IUploadInfoRepository
    {
        private readonly UploadInfoDbContext _uploadInfoDbContext;
        public DbUploadInfoRepository(UploadInfoDbContext uploadInfoDbContext)
        {
            _uploadInfoDbContext = uploadInfoDbContext;
        }

        public async Task<UploadInfoEntity> Insert(UploadInfoEntity entity)
        {
            _uploadInfoDbContext.UploadInfoEnties.Add(entity);
            await _uploadInfoDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<UploadInfoEntity?> GetOne(string lineCode, string sectionCode, int stationCode, string sn)
        {
            var uploadInfos = await _uploadInfoDbContext.UploadInfoEnties
                    .Where(e => e.LineCode.ToUpper().Equals(lineCode) && e.SectionCode.ToUpper().Equals(sectionCode) && e.StationCode == stationCode && e.Sn.ToUpper().Equals(sn)).ToListAsync();

            if (uploadInfos.Count() > 0)
            {
                return uploadInfos.OrderByDescending(e => e.UploadTime).First();
            }
            else
            {
                return null;
            }
        }
    }
}
