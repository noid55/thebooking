using Microsoft.AspNetCore.Mvc;
using thebooking.Models;
using Microsoft.EntityFrameworkCore;

namespace thebooking.Controllers;

public class RoomController : Controller
{
    private readonly ILogger<RoomController> _logger;
    private readonly BookingDbContext _bookingDbContext;

    public RoomController(BookingDbContext bookingDbContext, ILogger<RoomController> logger)
    {
        _bookingDbContext = bookingDbContext;
        _logger = logger;
    }

    public async Task<IActionResult> Table()
    {
        _logger.LogInformation("The study room table was accessed."); 

        List<Room> rooms = await _bookingDbContext.Rooms.ToListAsync();
        ViewBag.CurrentViewName = "List of Rooms";
        return View(rooms);
    }

    public async Task<IActionResult> Details(int id)
    {
        var room = await _bookingDbContext.Rooms.FirstOrDefaultAsync(i => i.RoomId == id);
        if (room == null)
        {
            _logger.LogWarning("Room " + id + " was not found.");
            return NotFound();
        }

        return View(room);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Room room)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _bookingDbContext.Rooms.Add(room);
                await _bookingDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Table));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while creating booking for room {RoomId}.", room.RoomId);
            ModelState.AddModelError(string.Empty, "An error occurred while creating booking for room " + room.RoomId + ". Please try again later."); 
        }

        return View(room);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var room = await _bookingDbContext.Rooms.FindAsync(id);
        if (room == null)
        {
            _logger.LogWarning("Room {RoomId} was not found.", id);
            return NotFound();
        }

        return View(room);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Room room)
    {
        if (id != room.RoomId)
        {
            _logger.LogWarning("Room ID mismatch. Route ID: {RoomId}, Room ID: {RoomId}", id, room.RoomId);
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _bookingDbContext.Rooms.Update(room);
                await _bookingDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Table));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while editing room booking {RoomId}.", room.RoomId);
                ModelState.AddModelError(string.Empty, "An error occurred while editing booking for room " + room.RoomId + ". Please try again later."); 
            }
        }

        return View(room);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _bookingDbContext.Rooms
            .FirstOrDefaultAsync(r => r.RoomId == id);
        if (room == null)
        {
            _logger.LogWarning("Room " + id + " was not found.");
            return NotFound();
        }

        return View(room);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var room = await _bookingDbContext.Rooms.FindAsync(id);
        if (room == null)
        {
            _logger.LogWarning("Room " + id + " was not found.");
            return NotFound();
        }

        try
        {
            _bookingDbContext.Rooms.Remove(room);
            await _bookingDbContext.SaveChangesAsync();
        } 
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting room booking {RoomId}.", room.RoomId);
            ModelState.AddModelError(string.Empty, "An error occurred while trying to delete booking for room " + room.RoomId + ". Please try again later."); 
            return View("Delete", room); 
        }
        return RedirectToAction(nameof(Table));
    }
}
