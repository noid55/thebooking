using thebooking.Models;

namespace thebooking.DAL;

public interface IEquipmentRepository
{
    Task<IEnumerable<Equipment>> GetAll();
    Task<Equipment?> GetEquipmentById(int id);
    Task Create(Equipment equipment);
    Task Update(Equipment equipment);
    Task<bool> Delete(int id);
}