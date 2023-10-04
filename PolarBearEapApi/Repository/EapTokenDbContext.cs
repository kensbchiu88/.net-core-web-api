using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace PolarBearEapApi.Repository
{
    public class EapTokenDbContext : DbContext
    {
        public EapTokenDbContext(DbContextOptions<EapTokenDbContext> options) : base(options)
        {
        }
        public DbSet<EapTokenEntity> EapTokenEntities {  get; set; }
    }
}
