using Microsoft.EntityFrameworkCore;
using thebooking.Models;

namespace thebooking.DAL;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _db;

    public BookingRepository(BookingDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Booking>> GetAll()
    {
        return await _db.Bookings.ToListAsync();
    }

    public async Task<Booking?> GetBookingById(int id)
    {
        return await _db.Bookings.FindAsync(id);
    }

    public async Task Create(Booking booking)
    {
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();
    }

    public async Task Update(Booking booking)
    {
        _db.Bookings.Update(booking);
        await _db.SaveChangesAsync();
    }

    public async Task<bool> Delete(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null)
        {
            return false;
        }

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();
        return true;
    }

    // Returnerer true hvis rommet allerede har en bekreftet booking som overlapper tidsrommet.
    // excludeBookingId brukes når en booking redigeres slik at den ikke kolliderer med seg selv.
    public async Task<bool> HasOverlap(int roomId, DateTime start, DateTime end, int? excludeBookingId = null)
    {
        return await _db.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            b.Status == BookingStatus.Confirmed &&
            b.StartTime < end &&
            start < b.EndTime &&
            (excludeBookingId == null || b.BookingId != excludeBookingId));
    }

    // Bekreftede bookinger for ett rom på en dag sortert på tid ( dette er til kalenderen).
    public async Task<IEnumerable<Booking>> GetBookingsForRoom(int roomId, DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        return await _db.Bookings
            .Where(b => b.RoomId == roomId &&
                        b.Status == BookingStatus.Confirmed &&
                        b.StartTime < dayEnd &&
                        b.EndTime > dayStart)
            .OrderBy(b => b.StartTime)
            .ToListAsync();
    }

    // alle bookingene til en person/student (til "mine bookinger").
    public async Task<IEnumerable<Booking>> GetBookingsForUser(string userId)
    {
        return await _db.Bookings
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.StartTime)
            .ToListAsync();
    }

    //Domeneregel: true hvis brukeren har en bekreftet booking som overlapper tidsrommet.
    public async Task<bool> HasUserOverlap(string userId, DateTime start, DateTime end, int? excludeBookingId = null)
    {
        return await _db.Bookings.AnyAsync(b =>
             b.UserId == userId &&
            b.Status == BookingStatus.Confirmed &&
            b.StartTime < end &&
            start < b.EndTime &&
            (excludeBookingId == null || b.BookingId != excludeBookingId)); //booking som redigeres sammelignes ikke med seg selv
    }
}