using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.Infra.Entities;

namespace PolarBearEapApi.Infra
{
    public class LearnFileAlterWarningDbContext : DbContext
    {
        public LearnFileAlterWarningDbContext(DbContextOptions<LearnFileAlterWarningDbContext> options) : base(options)
        {
        }

        public DbSet<LearnFileAlterWarningEntity> LearnFileAlterWarningEntities { get; set; }
    }
}
