using PolarBearEapApi.ApplicationCore.Entities;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IUploadInfoRepository
    {
        Task<UploadInfoEntity> Insert(UploadInfoEntity entity);
        Task<UploadInfoEntity?> GetOne(string lineCode, string sectionCode, int stationCode, string sn);
    }
}
