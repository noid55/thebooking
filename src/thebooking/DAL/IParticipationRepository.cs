using thebooking.Models;

namespace thebooking.DAL;

// Repository for participations (students who have joined a study session). All methods are async.
public interface IParticipationRepository
{
    Task<bool> IsParticipant(int bookingId, string userId);
    Task<int> CountForBooking(int bookingId);
    Task Create(Participation participation);
    Task<bool> Delete(int bookingId, string userId);
}