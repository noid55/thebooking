using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using thebooking.Controllers;
using thebooking.DAL;
using thebooking.Models;
using thebooking.Services;
using thebooking.ViewModels;

namespace thebooking.Tests.Controllers;

// Unit tests for BookingController. All dependencies are mocked, and a fake logged-in user is used.
public class BookingControllerTests
{
    private const string UserId = "user-1";

    private readonly IBookingRepository _mockBookingRepository = Substitute.For<IBookingRepository>();
    private readonly IRoomRepository _mockRoomRepository = Substitute.For<IRoomRepository>();
    private readonly IBuildingRepository _mockBuildingRepository = Substitute.For<IBuildingRepository>();
    private readonly IParticipationRepository _mockParticipationRepository = Substitute.For<IParticipationRepository>();
    private readonly IBookingService _mockBookingService = Substitute.For<IBookingService>();
    private readonly ILogger<BookingController> _mockLogger = Substitute.For<ILogger<BookingController>>();

    // Creates the controller with mocked dependencies and a logged-in test user
    private BookingController CreateController()
    {
        // UserManager is a class (not an interface), so the substitute needs its constructor arguments
        var mockUserManager = Substitute.For<UserManager<IdentityUser>>(
            Substitute.For<IUserStore<IdentityUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        mockUserManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns(UserId);

        // The room list is used for the dropdown when the form is shown again
        _mockRoomRepository.GetAll().Returns(new List<Room>());

        var controller = new BookingController(_mockBookingRepository, _mockRoomRepository, _mockBuildingRepository,
            _mockParticipationRepository, _mockBookingService, mockUserManager, _mockLogger);

        // A fake logged-in user, so User inside the controller is not null
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, UserId) }, "TestAuthentication"))
            }
        };
        return controller;
    }

    private static Booking CreateTestBooking()
    {
        var nextWeek = DateTime.Today.AddDays(7);
        return new Booking
        {
            RoomId = 1,
            Course = "data1500",
            Topic = "SQL joins",
            StartTime = nextWeek.AddHours(10),
            EndTime = nextWeek.AddHours(12),
            MaxParticipants = 4
        };
    }

    [Fact]
    public async Task Create_WhenBookingIsValid_RedirectsToMyBookings()
    {
        // Arrange: all domain rules pass
        var booking = CreateTestBooking();
        _mockBookingService.ValidateBooking(booking).Returns(new List<string>());
        var controller = CreateController();

        // Act
        var result = await controller.Create(booking);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("MyBookings", redirectResult.ActionName);
        Assert.Equal(UserId, booking.UserId);    // The booking belongs to the logged-in user
        Assert.Equal("DATA1500", booking.Course); // The course is stored in upper case
        await _mockBookingRepository.Received(1).Create(booking);
    }

    [Fact]
    public async Task Create_WhenDomainRuleFails_ReturnsViewWithBookingAndError()
    {
        // Arrange: BookingService reports that the room is already booked
        var booking = CreateTestBooking();
        _mockBookingService.ValidateBooking(booking)
            .Returns(new List<string> { "The room is already booked in this time period." });
        var controller = CreateController();

        // Act
        var result = await controller.Create(booking);

        // Assert: the form is shown again with the same booking, and nothing is saved
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewModel = Assert.IsAssignableFrom<BookingFormViewModel>(viewResult.ViewData.Model);
        Assert.Equal(booking, viewModel.Booking);
        Assert.False(controller.ModelState.IsValid);
        await _mockBookingRepository.DidNotReceive().Create(Arg.Any<Booking>());
    }
}