using thebooking.Models;

namespace thebooking.DAL;

//Repository for bookings (demo 6). All methods are async.
public interface IBookingRepository
{
    //CRUD
    Task<IEnumerable<Booking>> GetAll();
    Task<Booking?> GetBookingById(int id);
    Task Create(Booking booking);
    Task Update(Booking booking);
    Task<bool> Delete(int id);

    // Double booking checks (used by BookingService before a booking is saved).
    // Return true if there is a confirmed booking that overlaps the time period.
    // excludeBookingId is used when editing, so a booking is not compared with itself.
    Task<bool> HasOverlap(int roomId, DateTime start, DateTime end, int? excludeBookingId = null);
    Task<bool> HasUserOverlap(string userId, DateTime start, DateTime end, int? excludeBookingId = null);

    // Lists
    Task<IEnumerable<Booking>> GetBookingsForRoom(int roomId, DateTime date); //confirmed bookings for one room on one day (kalender)
    Task<IEnumerable<Booking>> GetBookingsForUser(string userId); // all bookings for one user (My bookings)
}