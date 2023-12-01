using PolarBearEapApi.Infra.Entities;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** UploadReport Repository */
    public interface IUploadReportRepository
    {
        Task<UploadReportEntity> Insert(UploadReportEntity entity);
    }
}
