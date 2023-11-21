using PolarBearEapApi.ApplicationCore.Entities;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IUploadReportRepository
    {
        Task<UploadReportEntity> Insert(UploadReportEntity entity);
    }
}
