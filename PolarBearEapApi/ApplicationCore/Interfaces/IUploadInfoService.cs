using PolarBearEapApi.ApplicationCore.Entities;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IUploadInfoService
    {
        UploadInfoEntity Insert(UploadInfoEntity entity);
        UploadInfoEntity? GetOne(string lineCode, string sectionCode, int stationCode, string sn);
    }
}
