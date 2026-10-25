using Microsoft.EntityFrameworkCore;
using thebooking.Models;

namespace thebooking.DAL;

public class BuildingRepository : IBuildingRepository
{
    private readonly BookingDbContext _db;

    public BuildingRepository(BookingDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Building>> GetAll()
    {
        return await _db.Buildings.ToListAsync();
    }

    public async Task<Building?> GetBuildingById(int id)
    {
        return await _db.Buildings.FindAsync(id);
    }

    public async Task Create(Building building)
    {
        _db.Buildings.Add(building);
        await _db.SaveChangesAsync();
    }

    public async Task Update(Building building)
    {
        _db.Buildings.Update(building);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var building = await _db.Buildings.FindAsync(id);
        if (building == null)
        {
            return false;
        }

        _db.Buildings.Remove(building);
        await _db.SaveChangesAsync();
        return true;
    }
}