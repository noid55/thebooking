using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using thebooking.DAL;
using thebooking.Models;
using thebooking.Services;

namespace thebooking.Controllers;

// Messages in a study session. Messages are shown on the Booking Details page,
// so after every action the user is sent back there.
[Authorize]
public class MessageController : Controller
{
    private readonly IMessageRepository _messageRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IBookingService _bookingService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<MessageController> _logger;

    public MessageController(IMessageRepository messageRepository, IBookingRepository bookingRepository,
        IBookingService bookingService, UserManager<IdentityUser> userManager, ILogger<MessageController> logger)
    {
        _messageRepository = messageRepository;
        _bookingRepository = bookingRepository;
        _bookingService = bookingService;
        _userManager = userManager;
        _logger = logger;
    }

    private string GetCurrentUserId()
    {
        return _userManager.GetUserId(User)!;
    }

    // The form on Booking Details uses the prefix "NewMessage" (from BookingDetailsViewModel)
    [HttpPost]
    public async Task<IActionResult> Create([Bind(Prefix = "NewMessage")] Message message)
    {
        var booking = await _bookingRepository.GetBookingById(message.BookingId);
        if (booking == null)
        {
            _logger.LogWarning("Booking {BookingId} was not found.", message.BookingId);
            return NotFound();
        }

        var userId = GetCurrentUserId();

        // Only the owner and the participants can leave messages
        if (!await _bookingService.CanPostMessage(booking, userId))
        {
            _logger.LogWarning("User {UserId} tried to post a message in booking {BookingId} without permission.", userId, booking.BookingId);
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "The message must be between 1 and 500 characters.";
            return RedirectToAction("Details", "Booking", new { id = message.BookingId });
        }

        try
        {
            message.UserId = userId;
            message.CreatedAt = DateTime.Now;
            await _messageRepository.Create(message);
            _logger.LogInformation("Message {MessageId} created in booking {BookingId}.", message.MessageId, message.BookingId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while creating a message in booking {BookingId}.", message.BookingId);
            TempData["Error"] = "The message could not be sent. Please try again later.";
        }

        return RedirectToAction("Details", "Booking", new { id = message.BookingId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var message = await _messageRepository.GetMessageById(id);
        if (message == null)
        {
            _logger.LogWarning("Message {MessageId} was not found.", id);
            return NotFound();
        }

        // Only the author can edit a message
        if (message.UserId != GetCurrentUserId())
        {
            _logger.LogWarning("User {UserId} tried to edit message {MessageId} without permission.", GetCurrentUserId(), id);
            return Forbid();
        }

        return View(message);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Message message)
    {
        if (id != message.MessageId)
        {
            _logger.LogWarning("Message ID mismatch. Route ID: {RouteId}, Message ID: {MessageId}", id, message.MessageId);
            return NotFound();
        }

        var existing = await _messageRepository.GetMessageById(id);
        if (existing == null)
        {
            _logger.LogWarning("Message {MessageId} was not found.", id);
            return NotFound();
        }

        if (existing.UserId != GetCurrentUserId())
        {
            _logger.LogWarning("User {UserId} tried to edit message {MessageId} without permission.", GetCurrentUserId(), id);
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Only the text can be changed (author, booking and time are kept)
                existing.Text = message.Text;
                await _messageRepository.Update(existing);
                _logger.LogInformation("Message {MessageId} was updated.", id);
                return RedirectToAction("Details", "Booking", new { id = existing.BookingId });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while editing message {MessageId}.", id);
                ModelState.AddModelError(string.Empty, "An error occurred while editing the message. Please try again later.");
            }
        }

        return View(message);
    }

    // The author or an admin can delete a message
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var message = await _messageRepository.GetMessageById(id);
        if (message == null)
        {
            _logger.LogWarning("Message {MessageId} was not found.", id);
            return NotFound();
        }

        if (message.UserId != GetCurrentUserId() && !User.IsInRole("Admin"))
        {
            _logger.LogWarning("User {UserId} tried to delete message {MessageId} without permission.", GetCurrentUserId(), id);
            return Forbid();
        }

        var bookingId = message.BookingId;
        try
        {
            await _messageRepository.Delete(id);
            _logger.LogInformation("Message {MessageId} was deleted.", id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting message {MessageId}.", id);
            TempData["Error"] = "The message could not be deleted. Please try again later.";
        }

        return RedirectToAction("Details", "Booking", new { id = bookingId });
    }
}