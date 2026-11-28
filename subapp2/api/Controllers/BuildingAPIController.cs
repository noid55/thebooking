using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using thebooking.DAL;
using thebooking.DTOs;
using thebooking.Models;

namespace thebooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildingAPIController : ControllerBase
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly ILogger<BuildingAPIController> _logger;

    // The repository comes in via dependency injection (registered in Program.cs)
    public BuildingAPIController(IBuildingRepository buildingRepository,
        ILogger<BuildingAPIController> logger)
    {
        _buildingRepository = buildingRepository;
        _logger = logger;
    }

    [HttpGet("buildinglist")]
    public async Task<IActionResult> BuildingList()
    {
        try
        {
            var buildings = await _buildingRepository.GetAll();
            var dtos = buildings.Select(b => new BuildingDto
            {
                BuildingId = b.BuildingId,
                Name = b.Name,
                Address = b.Address
            });
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BuildingAPIController] Failed to get the building list.");
            return StatusCode(500, "An error occurred while loading the buildings.");
        }
    }


    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] BuildingDto buildingDto)
    {
        if (buildingDto == null)
        {
            return BadRequest("Building data cannot be null");
        }

        // Convert the DTO to the entity that is saved in the database
        var building = new Building
        {
            Name = buildingDto.Name,
            Address = buildingDto.Address
        };

        // Runs the DataAnnotations on Building (StringLength on Name and Address)
        if (!TryValidateModel(building))
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _buildingRepository.Create(building);
            _logger.LogInformation("[BuildingAPIController] Building {BuildingId:0000} created.", building.BuildingId);

            var createdBuilding = new BuildingDto
            {
                BuildingId = building.BuildingId,
                Name = building.Name,
                Address = building.Address
            };
            return StatusCode(201, createdBuilding);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BuildingAPIController] Failed to create building {@building}", building);
            return StatusCode(500, "An error occurred while creating the building.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBuilding(int id)
    {
        try
        {
            var building = await _buildingRepository.GetBuildingById(id);
            if (building == null)
            {
                _logger.LogError("[BuildingAPIController] Building not found for the BuildingId {BuildingId:0000}", id);
                return NotFound("Building not found for the BuildingId");
            }

            var buildingDto = new BuildingDto
            {
                BuildingId = building.BuildingId,
                Name = building.Name,
                Address = building.Address
            };
            return Ok(buildingDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BuildingAPIController] Failed to get building {BuildingId:0000}", id);
            return StatusCode(500, "An error occurred while loading the building.");
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BuildingDto buildingDto)
    {
        if (buildingDto == null)
        {
            return BadRequest("Building data cannot be null");
        }

        // Find the building in the database
        var existingBuilding = await _buildingRepository.GetBuildingById(id);
        if (existingBuilding == null)
        {
            return NotFound("Building not found");
        }

        // Update the building properties
        existingBuilding.Name = buildingDto.Name;
        existingBuilding.Address = buildingDto.Address;

        // Same DataAnnotations check as in Create
        if (!TryValidateModel(existingBuilding))
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _buildingRepository.Update(existingBuilding);
            _logger.LogInformation("[BuildingAPIController] Building {BuildingId:0000} updated.", id);

            var updatedBuilding = new BuildingDto
            {
                BuildingId = existingBuilding.BuildingId,
                Name = existingBuilding.Name,
                Address = existingBuilding.Address
            };
            return Ok(updatedBuilding);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BuildingAPIController] Building update failed {@building}", existingBuilding);
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            bool returnOk = await _buildingRepository.Delete(id);
            if (!returnOk)
            {
                _logger.LogError("[BuildingAPIController] Building deletion failed for the BuildingId {BuildingId:0000}", id);
                return NotFound("Building not found");
            }

            _logger.LogInformation("[BuildingAPIController] Building {BuildingId:0000} deleted.", id);
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            // Room -> Building uses DeleteBehavior.Restrict (BookingDbContext),
            // so the database refuses to delete a building that still has rooms
            _logger.LogWarning(ex, "[BuildingAPIController] Building {BuildingId:0000} still has rooms and was not deleted.", id);
            return Conflict("The building cannot be deleted because it still has rooms. Delete or move its rooms first.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[BuildingAPIController] Failed to delete building {BuildingId:0000}", id);
            return StatusCode(500, "An error occurred while deleting the building.");
        }
    }
}