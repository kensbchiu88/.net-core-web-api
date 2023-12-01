using PolarBearEapApi.Infra.Entities;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** UploadInfo Repository */
    public interface IUploadInfoRepository
    {
        Task<UploadInfoEntity> Insert(UploadInfoEntity entity);
        Task<UploadInfoEntity?> GetOne(string lineCode, string sectionCode, int stationCode, string sn);
    }
}
