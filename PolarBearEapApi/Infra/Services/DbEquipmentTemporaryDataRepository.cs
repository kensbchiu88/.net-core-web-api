using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.Infra.Entities;

namespace PolarBearEapApi.Infra.Services
{
    public class DbEquipmentTemporaryDataRepository : IEquipmentTemporaryDataRepository
    {
        private readonly EquipmentTemporaryDataDbContext _context;

        public DbEquipmentTemporaryDataRepository(EquipmentTemporaryDataDbContext context)
        {
            _context = context;
        }

        public async Task<EquipmentTemporaryDataEntity> Insert(EquipmentTemporaryDataEntity entity)
        {
            _context.EquipmentTemporaryDataEntities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<EquipmentTemporaryDataEntity?> GetOne(string lineCode, string sn, string key)
        { 
            var entities = await _context.EquipmentTemporaryDataEntities
                .Where(e => e.LineCode.ToUpper().Equals(lineCode.ToUpper()) && e.Sn.ToUpper().Equals(sn.ToUpper()) && e.DataKey.ToUpper().Equals(key.ToUpper())).ToListAsync();

            if (entities.Count > 0)
            {
                return entities.OrderByDescending(e => e.CreateTime).First();
            }
            else
            {
                return null;
            }    
        }
    }
}
