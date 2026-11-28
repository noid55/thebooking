using thebooking.Models;

namespace thebooking.DAL;

public interface IBuildingRepository
{
    Task<IEnumerable<Building>> GetAll();
    Task<Building?> GetBuildingById(int id);
    Task Create(Building building);
    Task Update(Building building);
    Task<bool> Delete(int id);
}