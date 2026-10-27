using thebooking.DAL;
using thebooking.Models;

namespace thebooking.Services;

// Domenereglene for booking!
public class BookingService : IBookingService
{
    private static readonly TimeSpan OpeningTime = new(8, 0, 0);
    private static readonly TimeSpan ClosingTime = new(20, 0, 0);
    private static readonly TimeSpan MinDuration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<List<string>> ValidateBooking(Booking booking)
    {
        var errors = new List<string>();

        // Regel 1: sluttid etter starttid. Uten dette gir de andre tidsreglene ingen mening.
        if (booking.EndTime <= booking.StartTime)
        {
            errors.Add("The end time must be after the start time.");
            return errors;
        }

        // Regel 2: ikke bakover i tid
        if (booking.StartTime < DateTime.Now)
        {
            errors.Add("You cannot book a time that has already passed.");
        }

        // Regel 3: samme dag, innenfor åpningstiden 08:00–20:00
        if (booking.StartTime.Date != booking.EndTime.Date ||
            booking.StartTime.TimeOfDay < OpeningTime ||
            booking.EndTime.TimeOfDay > ClosingTime)
        {
            errors.Add("Rooms can only be booked between 08:00 and 20:00 on the same day.");
        }

        // Regel 4: varighet mellom 30 minutter og 4 timer
        var duration = booking.EndTime - booking.StartTime;
        if (duration < MinDuration || duration > MaxDuration)
        {
            errors.Add("A booking must last between 30 minutes and 4 hours.");
        }

        // Ved redigering skal bookingen ikke sammenlignes med seg selv
        int? excludeBookingId = booking.BookingId == 0 ? null : booking.BookingId;

        // Regel 5: rommet er ikke allerede booket i tidsrommet
        if (await _bookingRepository.HasOverlap(booking.RoomId, booking.StartTime, booking.EndTime, excludeBookingId))
        {
            errors.Add("The room is already booked in this time period.");
        }

        // Regel 6: studenten har ikke en annen booking i samme tidsrom
        if (await _bookingRepository.HasUserOverlap(booking.UserId, booking.StartTime, booking.EndTime, excludeBookingId))
        {
            errors.Add("You already have another booking in this time period.");
        }

        return errors;
    }
}