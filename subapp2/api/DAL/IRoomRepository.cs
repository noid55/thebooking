using thebooking.Models;

namespace thebooking.DAL;

public interface IRoomRepository
{
    Task<IEnumerable<Room>> Search(int? buildingId, int? minCapacity, List<int>? equipmentIds);
    //sjekke for like navn
    Task<bool> NameExistsInBuilding(string name, int buildingId, int? excludeRoomId = null);
    Task<IEnumerable<Room>> GetAll();
    Task<Room?> GetRoomById(int id);
    Task Create(Room room);
    Task Update(Room room);
    Task<bool> Delete(int id);
    Task UpdateEquipment(int roomId, List<int> equipmentIds);
}