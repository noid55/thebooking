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
    private readonly IBuildingRepository _buildingRepository; // Used for the building list in the search form
    private readonly IParticipationRepository _participationRepository;
    private readonly IBookingService _bookingService;
    private readonly UserManager<IdentityUser> _userManager; // Used to find the logged-in user
    private readonly ILogger<BookingController> _logger;

    public BookingController(IBookingRepository bookingRepository, IRoomRepository roomRepository,
        IBuildingRepository buildingRepository, IParticipationRepository participationRepository,
        IBookingService bookingService, UserManager<IdentityUser> userManager, ILogger<BookingController> logger)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _buildingRepository = buildingRepository;
        _participationRepository = participationRepository;
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

    // Only the owner of the booking or an admin can edit, cancel or delete it
    private bool CanManageBooking(Booking booking)
    {
        return booking.UserId == GetCurrentUserId() || User.IsInRole("Admin");
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

    // All bookings: only for admin (students use My bookings and Find study groups)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Table()
    {
        _logger.LogInformation("The booking table was accessed.");

        var bookings = await _bookingRepository.GetAll();
        var bookingsViewModel = new BookingsViewModel(bookings, "All Bookings");
        return View(bookingsViewModel);
    }

    // The bookings of the logged-in user (My bookings). Uses the same view as Table.
    public async Task<IActionResult> MyBookings()
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("User {UserId} opened their bookings.", userId);

        var bookings = await _bookingRepository.GetBookingsForUser(userId);
        var bookingsViewModel = new BookingsViewModel(bookings, "My Bookings");
        return View("Table", bookingsViewModel);
    }


    // Find study groups: open sessions that have not started, searchable by course, building and date
    public async Task<IActionResult> Open(string? course, int? buildingId, DateTime? date)
    {
        _logger.LogInformation("Study group search: course {Course}, building {BuildingId}, date {Date}.", course, buildingId, date);

        var sessions = await _bookingRepository.SearchOpenSessions(course, buildingId, date);
        var buildings = await _buildingRepository.GetAll();

        var viewModel = new OpenSessionsViewModel
        {
            Sessions = sessions,
            Course = course,
            BuildingId = buildingId,
            Date = date,
            BuildingSelectList = buildings.Select(b => new SelectListItem
            {
                Value = b.BuildingId.ToString(),
                Text = b.Name,
                Selected = b.BuildingId == buildingId
            }).ToList()
        };

        return View(viewModel);
    }

    // Shows the booking, the participants and the messages.
    // The ViewModel tells the view what the logged-in user is allowed to do.
    public async Task<IActionResult> Details(int id)
    {
        var booking = await _bookingRepository.GetBookingById(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} was not found.", id);
            return NotFound();
        }

        var userId = GetCurrentUserId();
        var isParticipant = await _participationRepository.IsParticipant(id, userId);

        // Private sessions can only be seen by the owner, an admin and students who have already joined
        if (!booking.IsOpen && !CanManageBooking(booking) && !isParticipant)
        {
            _logger.LogWarning("User {UserId} tried to open private booking {BookingId} without permission.", userId, id);
            return Forbid();
        }

        var joinErrors = await _bookingService.ValidateJoin(booking, userId);
        var canPostMessage = await _bookingService.CanPostMessage(booking, userId);

        var viewModel = new BookingDetailsViewModel
        {
            Booking = booking,
            CurrentUserId = userId,
            IsAdmin = User.IsInRole("Admin"),
            CanManage = CanManageBooking(booking),
            IsParticipant = isParticipant,
            CanJoin = joinErrors.Count == 0,
            JoinErrors = joinErrors,
            CanPostMessage = canPostMessage,
            // Messages are private: only the owner, the participants and an admin can read them
            CanSeeMessages = canPostMessage || User.IsInRole("Admin"),
            NewMessage = new Message { BookingId = id }
        };

        return View(viewModel);
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

        // Courses are stored in upper case, so "data1500" and "DATA1500" are the same in the search
        booking.Course = booking.Course.Trim().ToUpper();

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

        if (!CanManageBooking(booking))
        {
            _logger.LogWarning("User {UserId} tried to edit booking {BookingId} without permission.", GetCurrentUserId(), id);
            return Forbid();
        }

        if (!_bookingService.CanBeChanged(booking))
        {
            TempData["Error"] = "A cancelled booking, or a booking that has already started, cannot be edited.";
            return RedirectToAction(nameof(Details), new { id });
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

        // Checked on the server too, because a request can be sent without using the form
        if (!CanManageBooking(existing))
        {
            _logger.LogWarning("User {UserId} tried to edit booking {BookingId} without permission.", GetCurrentUserId(), id);
            return Forbid();
        }

        if (!_bookingService.CanBeChanged(existing))
        {
            TempData["Error"] = "A cancelled booking, or a booking that has already started, cannot be edited.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // The owner of the booking does not change when it is edited
        booking.UserId = existing.UserId;

        // Courses are stored in upper case, so "data1500" and "DATA1500" are the same in the search
        booking.Course = booking.Course.Trim().ToUpper();

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
                _logger.LogInformation("Booking {BookingId} was updated.", id);
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
    // The logged-in student joins an open study session
    [HttpPost]
    public async Task<IActionResult> Join(int id)
    {
        var booking = await _bookingRepository.GetBookingById(id);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} was not found.", id);
            return NotFound();
        }

        var userId = GetCurrentUserId();

        // The join rules are checked on the server, even though the button is hidden when joining is not allowed
        var errors = await _bookingService.ValidateJoin(booking, userId);
        if (errors.Count > 0)
        {
            _logger.LogWarning("User {UserId} could not join booking {BookingId}: {Errors}", userId, id, string.Join(" ", errors));
            TempData["Error"] = string.Join(" ", errors);
            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            await _participationRepository.Create(new Participation { BookingId = id, UserId = userId });
            _logger.LogInformation("User {UserId} joined booking {BookingId}.", userId, id);
            TempData["Success"] = "You have joined the study session.";
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user {UserId} tried to join booking {BookingId}.", userId, id);
            TempData["Error"] = "You could not join the session. Please try again later.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    // The logged-in student leaves a study session they have joined
    [HttpPost]
    public async Task<IActionResult> Leave(int id)
    {
        var userId = GetCurrentUserId();

        try
        {
            var removed = await _participationRepository.Delete(id, userId);
            if (removed)
            {
                _logger.LogInformation("User {UserId} left booking {BookingId}.", userId, id);
                TempData["Success"] = "You have left the study session.";
            }
            else
            {
                _logger.LogWarning("User {UserId} tried to leave booking {BookingId} without being a participant.", userId, id);
                TempData["Error"] = "You are not a participant in this session.";
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while user {UserId} tried to leave booking {BookingId}.", userId, id);
            TempData["Error"] = "You could not leave the session. Please try again later.";
        }

        return RedirectToAction(nameof(Details), new { id });
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

        if (!CanManageBooking(booking))
        {
            _logger.LogWarning("User {UserId} tried to cancel booking {BookingId} without permission.", GetCurrentUserId(), id);
            return Forbid();
        }

        if (!_bookingService.CanBeChanged(booking))
        {
            TempData["Error"] = "This booking is already cancelled or has already started.";
            return RedirectToAction(nameof(Details), new { id });
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

        if (!CanManageBooking(booking))
        {
            _logger.LogWarning("User {UserId} tried to delete booking {BookingId} without permission.", GetCurrentUserId(), id);
            return Forbid();
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

        if (!CanManageBooking(booking))
        {
            _logger.LogWarning("User {UserId} tried to delete booking {BookingId} without permission.", GetCurrentUserId(), id);
            return Forbid();
        }

        try
        {
            await _bookingRepository.Delete(id);
            _logger.LogInformation("Booking {BookingId} was deleted.", id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting booking {BookingId}.", id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the booking. Please try again later.");
            return View("Delete", booking);
        }
        return RedirectToAction(nameof(MyBookings));
    }
}