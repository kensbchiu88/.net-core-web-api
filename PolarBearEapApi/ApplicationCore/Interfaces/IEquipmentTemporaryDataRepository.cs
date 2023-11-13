using PolarBearEapApi.ApplicationCore.Entities;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    public interface IEquipmentTemporaryDataRepository
    {
        Task<EquipmentTemporaryDataEntity> Insert(EquipmentTemporaryDataEntity entity);
        Task<EquipmentTemporaryDataEntity?> GetOne(string lineCode, string sn, string key);
    }
}
