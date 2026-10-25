using Microsoft.EntityFrameworkCore;
using thebooking.Models;

namespace thebooking.DAL;

public class RoomRepository : IRoomRepository
{
    private readonly BookingDbContext _db;

    public RoomRepository(BookingDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Room>> GetAll()
    {
        return await _db.Rooms.ToListAsync();
    }

    public async Task<Room?> GetRoomById(int id)
    {
        return await _db.Rooms.FindAsync(id);
    }

    public async Task Create(Room room)
    {
        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();
    }

    public async Task Update(Room room)
    {
        _db.Rooms.Update(room);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room == null)
        {
            return false;
        }

        _db.Rooms.Remove(room);
        await _db.SaveChangesAsync();
        return true;
    }
    // filtrere på bygning, minimum kapasitet og utstyr (filter som ikke er fylt ut, blir ikke brukt)
    public async Task<IEnumerable<Room>> Search(int? buildingId, int? minCapacity, List<int>? equipmentIds)
    {
        IQueryable<Room> query = _db.Rooms;

        if (buildingId.HasValue)
        {
            query = query.Where(r => r.BuildingId == buildingId.Value);
        }
        if (minCapacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= minCapacity.Value);
        }
        if (equipmentIds != null)
        {
            foreach (var equipmentId in equipmentIds)
            {
                query = query.Where(r => r.RoomEquipments!.Any(re => re.EquipmentId == equipmentId));
            }
        }

        return await query.ToListAsync();
    }

    // true hvis bygget allerede har et rom med samme navn.
    // excludeRoomId brukes ved redigering så rommet ikke kolliderer med seg selv.
    public async Task<bool> NameExistsInBuilding(string name, int buildingId, int? excludeRoomId = null)
    {
        return await _db.Rooms.AnyAsync(r =>
            r.BuildingId == buildingId &&
            r.Name == name &&
            (excludeRoomId == null || r.RoomId != excludeRoomId));
    }

    // Setter utstyret til et rom: fjerner koblinger som ikke er valgt, legger til nye
    public async Task UpdateEquipment(int roomId, List<int> equipmentIds)
    {
        var existing = await _db.RoomEquipments
            .Where(re => re.RoomId == roomId)
            .ToListAsync();

        // fjern utstyr som ikke lenger er krysset av
        _db.RoomEquipments.RemoveRange(existing.Where(re => !equipmentIds.Contains(re.EquipmentId)));

        // legg til utstyr som er nytt
        var existingIds = existing.Select(re => re.EquipmentId).ToList();
        foreach (var equipmentId in equipmentIds.Where(id => !existingIds.Contains(id)))
        {
            _db.RoomEquipments.Add(new RoomEquipment { RoomId = roomId, EquipmentId = equipmentId });
        }

        await _db.SaveChangesAsync();
    }
}