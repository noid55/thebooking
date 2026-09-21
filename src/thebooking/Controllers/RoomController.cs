using Microsoft.AspNetCore.Mvc;
using thebooking.Models;

namespace thebooking.Controllers;

public class RoomController : Controller
{
    private readonly BookingDbContext _bookingDbContext;
    public RoomController(BookingDbContext bookingDbContext)
{
    _bookingDbContext = bookingDbContext;
}
    public IActionResult Table()
    { 
        List<Room> rooms = _bookingDbContext.Rooms.ToList();
        
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
}