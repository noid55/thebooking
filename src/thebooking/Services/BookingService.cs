using thebooking.DAL;
using thebooking.Models;

namespace thebooking.Services;

// The domain rules for booking a study session
public class BookingService : IBookingService
{
    private static readonly TimeSpan OpeningTime = new(8, 0, 0);
    private static readonly TimeSpan ClosingTime = new(20, 0, 0);
    private static readonly TimeSpan MinDuration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository; // Used to check the room's capacity

    public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
    }

    public async Task<List<string>> ValidateBooking(Booking booking)
    {
        var errors = new List<string>();

        // Rule 1: the end time must be after the start time. Without this, the other time rules make no sense.
        if (booking.EndTime <= booking.StartTime)
        {
            errors.Add("The end time must be after the start time.");
            return errors;
        }

        // Rule 2: no bookings back in time
        if (booking.StartTime < DateTime.Now)
        {
            errors.Add("You cannot book a time that has already passed.");
        }

        // Rule 3: same day, within opening hours 08:00–20:00
        if (booking.StartTime.Date != booking.EndTime.Date ||
            booking.StartTime.TimeOfDay < OpeningTime ||
            booking.EndTime.TimeOfDay > ClosingTime)
        {
            errors.Add("Rooms can only be booked between 08:00 and 20:00 on the same day.");
        }

        // Rule 4: duration between 30 minutes and 4 hours
        var duration = booking.EndTime - booking.StartTime;
        if (duration < MinDuration || duration > MaxDuration)
        {
            errors.Add("A booking must last between 30 minutes and 4 hours.");
        }

        // When editing, the booking should not be compared with itself
        int? excludeBookingId = booking.BookingId == 0 ? null : booking.BookingId;

        // Rule 5: the room is not already booked in this time period
        if (await _bookingRepository.HasOverlap(booking.RoomId, booking.StartTime, booking.EndTime, excludeBookingId))
        {
            errors.Add("The room is already booked in this time period.");
        }

        // Rule 6: the student does not have another booking in the same time period
        if (await _bookingRepository.HasUserOverlap(booking.UserId, booking.StartTime, booking.EndTime, excludeBookingId))
        {
            errors.Add("You already have another booking in this time period.");
        }

        // Rule 7: the room must exist, and the session cannot have more participants than the room has seats
        var room = await _roomRepository.GetRoomById(booking.RoomId);
        if (room == null)
        {
            errors.Add("The selected room does not exist.");
        }
        else if (booking.MaxParticipants > room.Capacity)
        {
            errors.Add($"Max participants cannot be higher than the room's capacity ({room.Capacity}).");
        }

        return errors;
    }     
    // Rule 8: a cancelled booking, or a booking that has already started, cannot be changed
    public bool CanBeChanged(Booking booking)
    {
        return booking.Status == BookingStatus.Confirmed && booking.StartTime > DateTime.Now;
    }
        
   
}