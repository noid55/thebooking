using thebooking.Models;

namespace thebooking.Services;

public interface IBookingService
{
    Task<List<string>> ValidateBooking(Booking booking);
}