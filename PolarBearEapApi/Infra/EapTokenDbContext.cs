using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PolarBearEapApi.ApplicationCore.Entities;

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
