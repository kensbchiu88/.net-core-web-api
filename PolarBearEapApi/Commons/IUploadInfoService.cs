using PolarBearEapApi.Repository;

namespace PolarBearEapApi.Commons
{
    public interface IUploadInfoService
    {
        UploadInfoEntity Insert(UploadInfoEntity entity);
        UploadInfoEntity? GetOne(string lineCode, string sectionCode, int stationCode, string sn);
    }
}
