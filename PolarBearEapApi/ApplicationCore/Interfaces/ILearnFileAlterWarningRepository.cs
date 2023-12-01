using PolarBearEapApi.Infra.Entities;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** LearnFileAlterWarning Repository */
    public interface ILearnFileAlterWarningRepository
    {
        Task<LearnFileAlterWarningEntity> Insert(LearnFileAlterWarningEntity entity);
    }
}
