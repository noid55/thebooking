using Microsoft.AspNetCore.Mvc;
using thebooking.DAL;
using thebooking.DTOs;
using thebooking.Models;

namespace thebooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentAPIController : ControllerBase
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly ILogger<EquipmentAPIController> _logger;

    public EquipmentAPIController(IEquipmentRepository equipmentRepository,
        ILogger<EquipmentAPIController> logger)
    {
        _equipmentRepository = equipmentRepository;
        _logger = logger;
    }

    [HttpGet("equipmentlist")]
    public async Task<IActionResult> EquipmentList()
    {
        try
        {
            var equipment = await _equipmentRepository.GetAll();
            var dtos = equipment.Select(e => new EquipmentDto
            {
                EquipmentId = e.EquipmentId,
                Name = e.Name
            });
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EquipmentAPIController] Failed to get the equipment list.");
            return StatusCode(500, "An error occurred while loading the equipment.");
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] EquipmentDto equipmentDto)
    {
        if (equipmentDto == null)
        {
            return BadRequest("Equipment data cannot be null");
        }

        var equipment = new Equipment { Name = equipmentDto.Name };

        // Runs the DataAnnotations on Equipment (StringLength on Name)
        if (!TryValidateModel(equipment))
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _equipmentRepository.Create(equipment);
            _logger.LogInformation("[EquipmentAPIController] Equipment {EquipmentId:0000} created.", equipment.EquipmentId);

            var created = new EquipmentDto { EquipmentId = equipment.EquipmentId, Name = equipment.Name };
            return StatusCode(201, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EquipmentAPIController] Failed to create equipment {@equipment}", equipment);
            return StatusCode(500, "An error occurred while creating the equipment.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEquipment(int id)
    {
        try
        {
            var equipment = await _equipmentRepository.GetEquipmentById(id);
            if (equipment == null)
            {
                _logger.LogError("[EquipmentAPIController] Equipment not found for the EquipmentId {EquipmentId:0000}", id);
                return NotFound("Equipment not found for the EquipmentId");
            }

            return Ok(new EquipmentDto { EquipmentId = equipment.EquipmentId, Name = equipment.Name });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EquipmentAPIController] Failed to get equipment {EquipmentId:0000}", id);
            return StatusCode(500, "An error occurred while loading the equipment.");
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EquipmentDto equipmentDto)
    {
        if (equipmentDto == null)
        {
            return BadRequest("Equipment data cannot be null");
        }

        var existing = await _equipmentRepository.GetEquipmentById(id);
        if (existing == null)
        {
            return NotFound("Equipment not found");
        }

        existing.Name = equipmentDto.Name;

        if (!TryValidateModel(existing))
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            await _equipmentRepository.Update(existing);
            _logger.LogInformation("[EquipmentAPIController] Equipment {EquipmentId:0000} updated.", id);
            return Ok(new EquipmentDto { EquipmentId = existing.EquipmentId, Name = existing.Name });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EquipmentAPIController] Equipment update failed {@equipment}", existing);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            bool returnOk = await _equipmentRepository.Delete(id);
            if (!returnOk)
            {
                _logger.LogError("[EquipmentAPIController] Equipment deletion failed for the EquipmentId {EquipmentId:0000}", id);
                return NotFound("Equipment not found");
            }

            _logger.LogInformation("[EquipmentAPIController] Equipment {EquipmentId:0000} deleted.", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EquipmentAPIController] Failed to delete equipment {EquipmentId:0000}", id);
            return StatusCode(500, "An error occurred while deleting the equipment.");
        }
    }
}