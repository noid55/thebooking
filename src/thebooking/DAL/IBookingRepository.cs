using thebooking.Models;

namespace thebooking.DAL;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAll();
    Task<Booking?> GetBookingById(int id);
    Task Create(Booking booking);
    Task Update(Booking booking);
    Task<bool> Delete(int id);

    // Dobbelbooking
    Task<bool> HasOverlap(int roomId, DateTime start, DateTime end, int? excludeBookingId = null);
    Task<IEnumerable<Booking>> GetBookingsForRoom(int roomId, DateTime date);
    Task<IEnumerable<Booking>> GetBookingsForUser(string userId);
}