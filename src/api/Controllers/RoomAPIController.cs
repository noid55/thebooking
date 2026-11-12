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
[HttpPost("create")]
public async Task<IActionResult> Create([FromBody] RoomDto roomDto)
{
    if (roomDto == null)
    {
        return BadRequest("Room data is missing.");
    }

    var room = new Room
    {
        Name = roomDto.Name,
        Capacity = roomDto.Capacity,
        Building = roomDto.Building,
        Description = roomDto.Description
    };

    if (!TryValidateModel(room))
    {
        return ValidationProblem(ModelState);
    }

    try
    {
        await _db.Rooms.AddAsync(room);
        await _db.SaveChangesAsync();

        var createdRoom = new RoomDto
        {
            RoomId = room.RoomId,
            Name = room.Name,
            Capacity = room.Capacity,
            Building = room.Building,
            Description = room.Description
        };

        return StatusCode(201, createdRoom);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to create room.");
        return StatusCode(500, "An error occurred while creating the room.");
    }
}
}


