using Microsoft.AspNetCore.Mvc;
using thebooking.DAL;
using thebooking.DTOs;
using thebooking.Models;
using thebooking.Services;

namespace thebooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingAPIController : ControllerBase
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingAPIController> _logger;

    public BookingAPIController(IBookingRepository bookingRepository,
        IBookingService bookingService,
        ILogger<BookingAPIController> logger)
    {
        _bookingRepository = bookingRepository;
        _bookingService = bookingService;
        _logger = logger;
    }

    // Until JWT (#6) every request acts as the seeded test user.
    // Later: read the user ID from the token instead.
    private static string GetCurrentUserId() => DBInit.TestUserId;

    private static BookingDto ToDto(Booking b) => new BookingDto
    {
        BookingId = b.BookingId,
        RoomId = b.RoomId,
        RoomName = b.Room?.Name,
        Course = b.Course,
        Topic = b.Topic,
        StartTime = b.StartTime,
        EndTime = b.EndTime,
        IsOpen = b.IsOpen,
        MaxParticipants = b.MaxParticipants,
        Status = b.Status.ToString()
    };

    [HttpGet("bookinglist")]
    public async Task<IActionResult> BookingList()
    {
        try
        {
            var bookings = await _bookingRepository.GetAll();
            return Ok(bookings.Select(ToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BookingAPIController] Failed to get the booking list.");
            return StatusCode(500, "An error occurred while loading the bookings.");
        }
    }

    [HttpGet("mybookings")]
    public async Task<IActionResult> MyBookings()
    {
        try
        {
            var bookings = await _bookingRepository.GetBookingsForUser(GetCurrentUserId());
            return Ok(bookings.Select(ToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BookingAPIController] Failed to get bookings for the current user.");
            return StatusCode(500, "An error occurred while loading your bookings.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        try
        {
            var booking = await _bookingRepository.GetBookingById(id);
            if (booking == null)
            {
                _logger.LogError("[BookingAPIController] Booking not found for the BookingId {BookingId:0000}", id);
                return NotFound("Booking not found for the BookingId");
            }
            return Ok(ToDto(booking));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BookingAPIController] Failed to get booking {BookingId:0000}", id);
            return StatusCode(500, "An error occurred while loading the booking.");
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] BookingDto bookingDto)
    {
        if (bookingDto == null)
        {
            return BadRequest("Booking data cannot be null");
        }

        var booking = new Booking
        {
            RoomId = bookingDto.RoomId,
            Course = bookingDto.Course,
            Topic = bookingDto.Topic,
            StartTime = bookingDto.StartTime,
            EndTime = bookingDto.EndTime,
            IsOpen = bookingDto.IsOpen,
            MaxParticipants = bookingDto.MaxParticipants,
            UserId = GetCurrentUserId()   // set by the server, never by the client
        };

        // Step 1: DataAnnotations on Booking (Course regex, Topic length, MaxParticipants range)
        if (!TryValidateModel(booking))
        {
            return ValidationProblem(ModelState);
        }

        // Step 2: domain rules (opening hours, duration, no overlap, capacity ...)
        var ruleErrors = await _bookingService.ValidateBooking(booking);
        foreach (var error in ruleErrors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
        if (ruleErrors.Count > 0)
        {
            _logger.LogWarning("[BookingAPIController] Booking rejected by domain rules: {Errors}", ruleErrors);
            return ValidationProblem(ModelState);
        }

        try
        {
            await _bookingRepository.Create(booking);
            _logger.LogInformation("[BookingAPIController] Booking {BookingId:0000} created.", booking.BookingId);
            return StatusCode(201, ToDto(booking));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BookingAPIController] Failed to create booking {@booking}", booking);
            return StatusCode(500, "An error occurred while creating the booking.");
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BookingDto bookingDto)
    {
        if (bookingDto == null)
        {
            return BadRequest("Booking data cannot be null");
        }

        var existing = await _bookingRepository.GetBookingById(id);
        if (existing == null)
        {
            return NotFound("Booking not found");
        }

        if (existing.UserId != GetCurrentUserId())
        {
            // Forbid() needs an auth scheme, which the API gets with JWT (#6)
            return StatusCode(403, "You can only change your own bookings.");
        }

        if (!_bookingService.CanBeChanged(existing))
        {
            return BadRequest("A cancelled booking, or a booking that has already started, cannot be changed.");
        }

        existing.RoomId = bookingDto.RoomId;
        existing.Course = bookingDto.Course;
        existing.Topic = bookingDto.Topic;
        existing.StartTime = bookingDto.StartTime;
        existing.EndTime = bookingDto.EndTime;
        existing.IsOpen = bookingDto.IsOpen;
        existing.MaxParticipants = bookingDto.MaxParticipants;

        if (!TryValidateModel(existing))
        {
            return ValidationProblem(ModelState);
        }

        // BookingId is set, so ValidateBooking doesn't compare the booking with itself
        var ruleErrors = await _bookingService.ValidateBooking(existing);
        foreach (var error in ruleErrors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
        if (ruleErrors.Count > 0)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _bookingRepository.Update(existing);
            _logger.LogInformation("[BookingAPIController] Booking {BookingId:0000} updated.", id);
            return Ok(ToDto(existing));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BookingAPIController] Booking update failed {@booking}", existing);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _bookingRepository.GetBookingById(id);
        if (booking == null)
        {
            return NotFound("Booking not found");
        }

        if (booking.UserId != GetCurrentUserId())
        {
            return StatusCode(403, "You can only delete your own bookings.");
        }

        try
        {
            // Participations and Messages are removed too (Cascade in BookingDbContext)
            await _bookingRepository.Delete(id);
            _logger.LogInformation("[BookingAPIController] Booking {BookingId:0000} deleted.", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BookingAPIController] Failed to delete booking {BookingId:0000}", id);
            return StatusCode(500, "An error occurred while deleting the booking.");
        }
    }
}