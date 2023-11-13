using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.ApplicationCore.Entities;

namespace PolarBearEapApi.Infra
{
    public class EquipmentTemporaryDataDbContext : DbContext
    {
        public EquipmentTemporaryDataDbContext(DbContextOptions<EquipmentTemporaryDataDbContext> options) : base(options)
        {
        }

        public DbSet<EquipmentTemporaryDataEntity> EquipmentTemporaryDataEntities { get; set; }
    }
}
