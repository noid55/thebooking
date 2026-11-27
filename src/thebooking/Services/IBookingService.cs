using thebooking.Models;

namespace thebooking.Services;

public interface IBookingService
{
 // Checks all domain rules and returns a list of error messages (empty list = valid)
    Task<List<string>> ValidateBooking(Booking booking);

// A booking can only be edited or cancelled if it is confirmed and has not started yet
    bool CanBeChanged(Booking booking);
}