using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using thebooking.DAL;
using thebooking.Models;
using thebooking.Services;
using thebooking.ViewModels;

namespace thebooking.Controllers;

// All booking pages require a logged-in user, because every booking belongs to a user
[Authorize]
public class BookingController : Controller
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingService _bookingService;
    private readonly UserManager<IdentityUser> _userManager; // Used to find the logged-in user
    private readonly ILogger<BookingController> _logger;

    public BookingController(IBookingRepository bookingRepository, IRoomRepository roomRepository,
        IBookingService bookingService, UserManager<IdentityUser> userManager, ILogger<BookingController> logger)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _bookingService = bookingService;
        _userManager = userManager;
        _logger = logger;
    }

    // Returns the ID of the logged-in user.
    // [Authorize] on the controller guarantees that someone is logged in here.
    private string GetCurrentUserId()
    {
        return _userManager.GetUserId(User)!;
    }

    // The ViewModel with the booking and the list of rooms for the dropdown
    private async Task<BookingFormViewModel> CreateBookingFormViewModel(Booking booking)
    {
        var rooms = await _roomRepository.GetAll();
        return new BookingFormViewModel
        {
            Booking = booking,
            RoomSelectList = rooms.Select(room => new SelectListItem
            {
                Value = room.RoomId.ToString(),
                Text = $"{room.Name} ({room.Building?.Name}, {room.Capacity} seats)"
            }).ToList()
        };
    }

    // All bookings (for admin)
    public async Task<IActionResult> Table()
    {
        _logger.LogInformation("The booking table was accessed.");

        var bookings = await _bookingRepository.GetAll();
        ViewBag.CurrentViewName = "All Bookings";
        return View(bookings);
    }

    // The bookings of the logged-in user (My bookings). Uses the same view as Table.
    public async Task<IActionResult> MyBookings()
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("User {UserId} opened their bookings.", userId);

        var bookings = await _bookingRepository.GetBookingsForUser(userId);
        ViewBag.CurrentViewName = "My Bookings";
        return View("Table", bookings);
    }

    public async Task<IActionResult> Details(int id)
    {
        var booking = await _bookingRepository.GetBookingById(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} was not found.", id);
            return NotFound();
        }

        return View(booking);
    }

    // roomId is optional: it comes from the "Book this room" button on Room Details
    [HttpGet]
    public async Task<IActionResult> Create(int? roomId)
    {
        var start = DateTime.Today.AddDays(1).AddHours(10); // Suggestion: tomorrow at 10:00
        var booking = new Booking
        {
            RoomId = roomId ?? 0,
            StartTime = start,
            EndTime = start.AddHours(1)
        };
        return View(await CreateBookingFormViewModel(booking));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Booking booking)
    {
        booking.UserId = GetCurrentUserId();

        // The domain rules are only checked when the fields themselves are valid
        if (ModelState.IsValid)
        {
            var errors = await _bookingService.ValidateBooking(booking);
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _bookingRepository.Create(booking);
                _logger.LogInformation("Booking {BookingId} created for room {RoomId}.", booking.BookingId, booking.RoomId);
                return RedirectToAction(nameof(MyBookings));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating a booking for room {RoomId}.", booking.RoomId);
                ModelState.AddModelError(string.Empty, "An error occurred while creating the booking. Please try again later.");
            }
        }

        return View(await CreateBookingFormViewModel(booking));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var booking = await _bookingRepository.GetBookingById(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} was not found.", id);
            return NotFound();
        }

        return View(await CreateBookingFormViewModel(booking));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Booking booking)
    {
        if (id != booking.BookingId)
        {
            _logger.LogWarning("Booking ID mismatch. Route ID: {RouteId}, Booking ID: {BookingId}", id, booking.BookingId);
            return NotFound();
        }

        var existing = await _bookingRepository.GetBookingById(id);
        if (existing == null)
        {
            _logger.LogWarning("Booking {BookingId} was not found.", id);
            return NotFound();
        }

        // The owner of the booking does not change when it is edited
        booking.UserId = existing.UserId;

        if (ModelState.IsValid)
        {
            var errors = await _bookingService.ValidateBooking(booking);
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Only the editable fields are copied over (CreatedAt, UserId and Status are kept)
                existing.RoomId = booking.RoomId;
                existing.Course = booking.Course;
                existing.Topic = booking.Topic;
                existing.StartTime = booking.StartTime;
                existing.EndTime = booking.EndTime;
                existing.IsOpen = booking.IsOpen;
                existing.MaxParticipants = booking.MaxParticipants;

                await _bookingRepository.Update(existing);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while editing booking {BookingId}.", id);
                ModelState.AddModelError(string.Empty, "An error occurred while editing the booking. Please try again later.");
            }
        }

        return View(await CreateBookingFormViewModel(booking));
    }

    // The booking is set to Cancelled; the row itself is not deleted
    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var booking = await _bookingRepository.GetBookingById(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} was not found.", id);
            return NotFound();
        }

        try
        {
            booking.Status = BookingStatus.Cancelled;
            await _bookingRepository.Update(booking);
            _logger.LogInformation("Booking {BookingId} was cancelled.", id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while cancelling booking {BookingId}.", id);
            TempData["Error"] = "The booking could not be cancelled. Please try again later.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var booking = await _bookingRepository.GetBookingById(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} was not found.", id);
            return NotFound();
        }

        return View(booking);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _bookingRepository.GetBookingById(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} was not found.", id);
            return NotFound();
        }

        try
        {
            await _bookingRepository.Delete(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting booking {BookingId}.", id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the booking. Please try again later.");
            return View("Delete", booking);
        }
        return RedirectToAction(nameof(Table));
    }
}