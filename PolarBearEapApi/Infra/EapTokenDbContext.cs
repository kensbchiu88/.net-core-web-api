using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.Infra.Entities;

namespace PolarBearEapApi.Infra
{
    public class EapTokenDbContext : DbContext
    {
        public EapTokenDbContext(DbContextOptions<EapTokenDbContext> options) : base(options)
        {
        }
        public DbSet<EapTokenEntity> EapTokenEntities { get; set; }
    }
}
