using PolarBearEapApi.Repository;

namespace PolarBearEapApi.Commons
{
    public class DbUploadInfoService : IUploadInfoService
    {
        private readonly UploadInfoDbContext _uploadInfoDbContext;
        public DbUploadInfoService(UploadInfoDbContext uploadInfoDbContext) 
        {
            _uploadInfoDbContext = uploadInfoDbContext;
        }

        public UploadInfoEntity Insert(UploadInfoEntity entity)
        {
            _uploadInfoDbContext.UploadInfoEnties.Add(entity);
            _uploadInfoDbContext.SaveChangesAsync();
            return entity;
        }

        public UploadInfoEntity? GetOne(string lineCode, string sectionCode, int stationCode, string sn)
        {
            var uploadInfos = _uploadInfoDbContext.UploadInfoEnties
                    .Where(e => e.LineCode.ToUpper().Equals(lineCode) && e.SectionCode.ToUpper().Equals(sectionCode) && e.StationCode == stationCode && e.Sn.ToUpper().Equals(sn));

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
