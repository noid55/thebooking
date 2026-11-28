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


    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoom(int id)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room == null)
        {
            _logger.LogError("[RoomAPIController] Room with ID {RoomId} not found.", id);
            return NotFound($"Room with ID {id} not found.");
        }

        // Return a DTO, not the entity, so lazy loading doesn't pull in Bookings
        return Ok(ToDto(room));
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] RoomDto roomDto)
    {
        if (roomDto == null)
        {
            return BadRequest("Room data is missing.");
        }
        
        //Find item in database
        var existingRoom = await _db.Rooms.FindAsync(id);
        if (existingRoom == null)
        {
            return NotFound("Room not found.");
        }
        
        //Update item properties
        existingRoom.Name = roomDto.Name;
        existingRoom.Capacity = roomDto.Capacity;
        existingRoom.Building = roomDto.Building;
        existingRoom.Description = roomDto.Description;

        // Validation rules live on the Room entity, same as in Create
        if (!TryValidateModel(existingRoom))
        {
            return ValidationProblem(ModelState);
        }

        //Save changes. SaveChangesAsync returns 0 when nothing changed,
        //which is not an error, so don't treat 0 rows as a failure.
        try
        {
            await _db.SaveChangesAsync();
            return Ok(ToDto(existingRoom));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[RoomAPIController] Failed to update room with ID {RoomId}.", id);
            return StatusCode(500, "An error occurred while updating the room.");
        }
    }

    private static RoomDto ToDto(Room room) => new RoomDto
    {
        RoomId = room.RoomId,
        Name = room.Name,
        Capacity = room.Capacity,
        Building = room.Building,
        Description = room.Description
    };

}