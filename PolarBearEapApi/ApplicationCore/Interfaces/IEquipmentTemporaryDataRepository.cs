using PolarBearEapApi.Infra.Entities;

namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** EquipmentTemporaryData Repository */
    public interface IEquipmentTemporaryDataRepository
    {
        Task<EquipmentTemporaryDataEntity> Insert(EquipmentTemporaryDataEntity entity);
        Task<EquipmentTemporaryDataEntity?> GetOne(string lineCode, string sn, string key);
    }
}
