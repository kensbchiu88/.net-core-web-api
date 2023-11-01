using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Interfaces;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** LearnFileAlterWarning Application Service */
    public class LearnFileAlterWarningService : ILearnFileAlterWarningService
    {
        private readonly ILearnFileAlterWarningRepository _repository;
        private readonly IEmailService _emailService;

        public LearnFileAlterWarningService(ILearnFileAlterWarningRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
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

            //Todo add send email
            //await _emailService.Send("ken.sp.chiu@fit-foxconn.com", "學習庫修改測試", "學習庫修改測試");            
        }        
    }
}
