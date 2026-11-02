using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using thebooking.DTOs;
using thebooking.Models;

namespace thebooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomAPIController : ControllerBase
{
    private readonly BookingDbContext _db;
    private readonly ILogger<RoomAPIController> _logger;

    public RoomAPIController(BookingDbContext db, ILogger<RoomAPIController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet("roomlist")]
    public async Task<IActionResult> RoomList()
    {
        var rooms = await _db.Rooms.ToListAsync();
        var dtos = rooms.Select(r => new RoomDto
        {
            RoomId = r.RoomId,
            Name = r.Name,
            Capacity = r.Capacity,
            Building = r.Building,
            Description = r.Description
        });
        return Ok(dtos);
    }
}
