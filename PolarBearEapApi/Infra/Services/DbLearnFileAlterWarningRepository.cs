using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Interfaces;

namespace PolarBearEapApi.Infra.Services
{
    public class DbLearnFileAlterWarningRepository : ILearnFileAlterWarningRepository
    {
        private readonly LearnFileAlterWarningDbContext _context;  

        public DbLearnFileAlterWarningRepository(LearnFileAlterWarningDbContext context)
        {
            _context = context;
        }
    
        public async Task<LearnFileAlterWarningEntity> Insert(LearnFileAlterWarningEntity entity)
        {
            _context.LearnFileAlterWarningEntities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
