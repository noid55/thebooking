using Microsoft.EntityFrameworkCore;
using thebooking.Models;

namespace thebooking.DAL;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly BookingDbContext _db;

    public EquipmentRepository(BookingDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Equipment>> GetAll()
    {
        return await _db.Equipment.ToListAsync();
    }

    public async Task<Equipment?> GetEquipmentById(int id)
    {
        return await _db.Equipment.FindAsync(id);
    }

    public async Task Create(Equipment equipment)
    {
        _db.Equipment.Add(equipment);
        await _db.SaveChangesAsync();
    }

    public async Task Update(Equipment equipment)
    {
        _db.Equipment.Update(equipment);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var equipment = await _db.Equipment.FindAsync(id);
        if (equipment == null)
        {
            return false;
        }

        _db.Equipment.Remove(equipment);
        await _db.SaveChangesAsync();
        return true;
    }
}