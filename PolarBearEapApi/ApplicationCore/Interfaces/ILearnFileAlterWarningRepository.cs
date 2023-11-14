using PolarBearEapApi.ApplicationCore.Entities;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface ILearnFileAlterWarningRepository
    {
        Task<LearnFileAlterWarningEntity> Insert(LearnFileAlterWarningEntity entity);
    }
}
