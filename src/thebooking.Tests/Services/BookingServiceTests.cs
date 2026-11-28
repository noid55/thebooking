using NSubstitute;
using thebooking.DAL;
using thebooking.Models;
using thebooking.Services;

namespace thebooking.Tests.Services;

// Unit tests for the domain rules in BookingService.
// The repositories are mocked with NSubstitute, so no real database is used.
public class BookingServiceTests
{
    private readonly IBookingRepository _mockBookingRepository = Substitute.For<IBookingRepository>();
    private readonly IRoomRepository _mockRoomRepository = Substitute.For<IRoomRepository>();
    private readonly IParticipationRepository _mockParticipationRepository = Substitute.For<IParticipationRepository>();
    private readonly BookingService _bookingService;

    // All test bookings are one week ahead, so rule 2 (not back in time) never fails by accident
    private static readonly DateTime NextWeek = DateTime.Today.AddDays(7);

    // xUnit creates a new instance of this class for every test, so every test gets fresh mocks
    public BookingServiceTests()
    {
        // Room 1 has 6 seats. Mocked methods that are not set up return false or 0 (no overlap, no participants).
        _mockRoomRepository.GetRoomById(1).Returns(new Room { RoomId = 1, Name = "Grupperom 102", Capacity = 6 });
        _bookingService = new BookingService(_mockBookingRepository, _mockRoomRepository, _mockParticipationRepository);
    }

    // A valid, open study session from 10:00 to 12:00 next week, owned by "owner-1"
    private static Booking CreateTestBooking()
    {
        return new Booking
        {
            BookingId = 1,
            RoomId = 1,
            UserId = "owner-1",
            Course = "DATA1500",
            Topic = "SQL joins",
            StartTime = NextWeek.AddHours(10),
            EndTime = NextWeek.AddHours(12),
            MaxParticipants = 4,
            IsOpen = true,
            Status = BookingStatus.Confirmed
        };
    }

    // Positive tests

    [Fact]
    public async Task ValidateBooking_WhenBookingIsValid_ReturnsNoErrors()
    {
        // Arrange
        var booking = CreateTestBooking();

        // Act
        var errors = await _bookingService.ValidateBooking(booking);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public async Task ValidateJoin_WhenSessionIsOpenAndHasSpace_ReturnsNoErrors()
    {
        // Arrange: one student has already joined, so 2 of 4 places are taken
        var booking = CreateTestBooking();
        _mockParticipationRepository.CountForBooking(booking.BookingId).Returns(1);

        // Act
        var errors = await _bookingService.ValidateJoin(booking, "student-2");

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void CanBeChanged_WhenBookingIsConfirmedAndInTheFuture_ReturnsTrue()
    {
        // Arrange
        var booking = CreateTestBooking();

        // Act
        var result = _bookingService.CanBeChanged(booking);

        // Assert
        Assert.True(result);
    }

    // Negative tests

    [Fact]
    public async Task ValidateBooking_WhenRoomIsAlreadyBooked_ReturnsOverlapError()
    {
        // Arrange: the repository reports that the room already has a booking in this time period
        var booking = CreateTestBooking();
        _mockBookingRepository
            .HasOverlap(booking.RoomId, booking.StartTime, booking.EndTime, Arg.Any<int?>())
            .Returns(true);

        // Act
        var errors = await _bookingService.ValidateBooking(booking);

        // Assert
        Assert.Contains("The room is already booked in this time period.", errors);
    }

    [Fact]
    public async Task ValidateBooking_WhenOutsideOpeningHours_ReturnsOpeningHoursError()
    {
        // Arrange: 19:00 to 21:00 ends after closing time (20:00)
        var booking = CreateTestBooking();
        booking.StartTime = NextWeek.AddHours(19);
        booking.EndTime = NextWeek.AddHours(21);

        // Act
        var errors = await _bookingService.ValidateBooking(booking);

        // Assert
        Assert.Contains("Rooms can only be booked between 08:00 and 20:00 on the same day.", errors);
    }

    [Fact]
    public async Task ValidateBooking_WhenMaxParticipantsIsHigherThanCapacity_ReturnsCapacityError()
    {
        // Arrange: room 1 has 6 seats, but the booking asks for 10 participants
        var booking = CreateTestBooking();
        booking.MaxParticipants = 10;

        // Act
        var errors = await _bookingService.ValidateBooking(booking);

        // Assert
        Assert.Contains("Max participants cannot be higher than the room's capacity (6).", errors);
    }

    [Fact]
    public async Task ValidateJoin_WhenSessionIsFull_ReturnsFullError()
    {
        // Arrange: 3 participants + the owner = 4 of 4 places taken
        var booking = CreateTestBooking();
        _mockParticipationRepository.CountForBooking(booking.BookingId).Returns(3);

        // Act
        var errors = await _bookingService.ValidateJoin(booking, "student-2");

        // Assert
        Assert.Contains("This session is full.", errors);
    }

    [Fact]
    public async Task ValidateJoin_WhenUserIsOwner_ReturnsOwnSessionError()
    {
        // Arrange
        var booking = CreateTestBooking();

        // Act: the owner tries to join their own session
        var errors = await _bookingService.ValidateJoin(booking, "owner-1");

        // Assert
        var error = Assert.Single(errors);
        Assert.Equal("You cannot join your own session.", error);
    }
}