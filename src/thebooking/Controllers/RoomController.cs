using Microsoft.AspNetCore.Mvc;
using thebooking.Models;
using Microsoft.EntityFrameworkCore;

namespace thebooking.Controllers;

public class RoomController : Controller
{
    private readonly BookingDbContext _bookingDbContext;

    public RoomController(BookingDbContext bookingDbContext)
    {
        _bookingDbContext = bookingDbContext;
    }

    public async Task<IActionResult> Table()
    {
        List<Room> rooms = await _bookingDbContext.Rooms.ToListAsync();
        ViewBag.CurrentViewName = "List of Rooms";
        return View(rooms);
    }

    public async Task<IActionResult> Details(int id)
    {
        var room = await _bookingDbContext.Rooms.FirstOrDefaultAsync(i => i.RoomId == id);
        if (room == null)
        {
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
        if (ModelState.IsValid)
        {
            _bookingDbContext.Rooms.Add(room);
            await _bookingDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Table));
        }

        return View(room);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var room = await _bookingDbContext.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        return View(room);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Room room)
    {
        if (id != room.RoomId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _bookingDbContext.Rooms.Update(room);
            await _bookingDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Table));
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
            return NotFound();
        }

        _bookingDbContext.Rooms.Remove(room);
        await _bookingDbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Table));
    }
}