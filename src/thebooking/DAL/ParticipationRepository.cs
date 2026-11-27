using Microsoft.EntityFrameworkCore;
using thebooking.Models;

namespace thebooking.DAL;

public class ParticipationRepository : IParticipationRepository
{
    private readonly BookingDbContext _db;

    public ParticipationRepository(BookingDbContext db)
    {
        _db = db;
    }

    // true if the user has already joined the session
    public async Task<bool> IsParticipant(int bookingId, string userId)
    {
        return await _db.Participations.AnyAsync(p => p.BookingId == bookingId && p.UserId == userId);
    }

    // Number of students who have joined the session (the owner is not included)
    public async Task<int> CountForBooking(int bookingId)
    {
        return await _db.Participations.CountAsync(p => p.BookingId == bookingId);
    }

    public async Task Create(Participation participation)
    {
        _db.Participations.Add(participation);
        await _db.SaveChangesAsync();
    }

    // Removes the user from the session. Returns false if the user was not a participant.
    public async Task<bool> Delete(int bookingId, string userId)
    {
        var participation = await _db.Participations
            .FirstOrDefaultAsync(p => p.BookingId == bookingId && p.UserId == userId);
        if (participation == null)
        {
            return false;
        }

        _db.Participations.Remove(participation);
        await _db.SaveChangesAsync();
        return true;
    }
}