using thebooking.Models;

namespace thebooking.ViewModels;

// The bookings to show in the booking table, plus the page title (All Bookings or My Bookings)
public class BookingsViewModel
{
    public IEnumerable<Booking> Bookings;
    public string? CurrentViewName;

    public BookingsViewModel(IEnumerable<Booking> bookings, string? currentViewName)
    {
        Bookings = bookings;
        CurrentViewName = currentViewName;
    }
}