using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.Infra.Entities;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** LearnFileAlterWarning Application Service。 學習庫異動通知*/
    public class LearnFileAlterWarningService : ILearnFileAlterWarningService
    {
        private readonly ILearnFileAlterWarningRepository _repository;

        public LearnFileAlterWarningService(ILearnFileAlterWarningRepository repository)
        {
            _repository = repository;
        }

        public async Task Send(string filePath, string alterTime, string equipment)
        {
            var alterTimeDateTime = DateTime.Parse(alterTime);

            LearnFileAlterWarningEntity entity = new LearnFileAlterWarningEntity();
            entity.FilePath = filePath;
            entity.AlterTime = alterTimeDateTime;
            entity.Equipment = equipment;
            entity.CreateTime = DateTime.Now;

            await _repository.Insert(entity);          
        }        
    }
}
