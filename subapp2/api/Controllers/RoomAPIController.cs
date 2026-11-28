using Microsoft.AspNetCore.Mvc;
using thebooking.DAL;
using thebooking.DTOs;
using thebooking.Models;

namespace thebooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomAPIController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly ILogger<RoomAPIController> _logger;

    // Repositoriene kommer inn via dependency injection (registrert i Program.cs)
    public RoomAPIController(IRoomRepository roomRepository,
                             IBuildingRepository buildingRepository,
                             ILogger<RoomAPIController> logger)
    {
        _roomRepository = roomRepository;
        _buildingRepository = buildingRepository;
        _logger = logger;
    }

    [HttpGet("roomlist")]
    public async Task<IActionResult> RoomList()
    {
        var rooms = await _roomRepository.GetAll();
        var dtos = rooms.Select(r => new RoomDto
        {
            RoomId = r.RoomId,
            Name = r.Name,
            Capacity = r.Capacity,
            BuildingId = r.BuildingId,
            BuildingName = r.Building?.Name,
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
            BuildingId = roomDto.BuildingId,   // bare fremmednøkkelen, ikke Building-objektet
            Description = roomDto.Description
        };

        if (!TryValidateModel(room))
        {
            return ValidationProblem(ModelState);
        }

        // Bygget må finnes, ellers feiler fremmednøkkelen i databasen
        var building = await _buildingRepository.GetBuildingById(room.BuildingId);
        if (building == null)
        {
            ModelState.AddModelError(nameof(RoomDto.BuildingId), "The selected building does not exist.");
            return ValidationProblem(ModelState);
        }

        // Samme regel som i MVC: to rom i samme bygg kan ikke ha samme navn
        if (await _roomRepository.NameExistsInBuilding(room.Name, room.BuildingId))
        {
            ModelState.AddModelError(nameof(RoomDto.Name), "A room with this name already exists in this building.");
            return ValidationProblem(ModelState);
        }

        try
        {
            await _roomRepository.Create(room);

            var createdRoom = new RoomDto
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Capacity = room.Capacity,
                BuildingId = room.BuildingId,
                BuildingName = building.Name,
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