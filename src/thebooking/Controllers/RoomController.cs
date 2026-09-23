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
        /*
        var rooms = new List<Room>();
        var room1 = new Room();
        room1.RoomId = 1;
        room1.Name = "PH170";
        room1.Capacity = 176;
        room1.Building = "P35";
        room1.Description = "Stort rom med mange plasser";
        

        var room2 = new Room
        {
            RoomId = 2, Name = "PH330", Capacity = 123, Building = "P35", Description = "Litt mindre room, to lerret"
        };
        
        var room3 = new Room
        {
            RoomId = 3, Name = "PH67", Capacity = 67, Building = "P67", Description = "SIX SEVEN 🤪"
        };
        
        rooms.Add(room1);
        rooms.Add(room2);
        rooms.Add(room3);
        */

     
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
        if (ModelState.IsValid){
            
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
   var room = await _bookingDbContext.Rooms.FindAsync(id);
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