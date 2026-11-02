using Microsoft.AspNetCore.Mvc;
using thebooking.DAL;
using thebooking.Models;

namespace thebooking.Controllers;

public class EquipmentController : Controller
{
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly ILogger<EquipmentController> _logger;

    public EquipmentController(IEquipmentRepository equipmentRepository, ILogger<EquipmentController> logger)
    {
        _equipmentRepository = equipmentRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Table()
    {
        _logger.LogInformation("The equipment table was accessed.");

        var equipment = await _equipmentRepository.GetAll();
        ViewBag.CurrentViewName = "List of Equipment";
        return View(equipment);
    }

    public async Task<IActionResult> Details(int id)
    {
        var equipment = await _equipmentRepository.GetEquipmentById(id);
        if (equipment == null)
        {
            _logger.LogWarning("Equipment {EquipmentId} was not found.", id);
            return NotFound();
        }

        return View(equipment);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Equipment equipment)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _equipmentRepository.Create(equipment);
                return RedirectToAction(nameof(Table));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating equipment {EquipmentName}.", equipment.Name);
                ModelState.AddModelError(string.Empty, "An error occurred while creating the equipment. Please try again later.");
            }
        }

        return View(equipment);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var equipment = await _equipmentRepository.GetEquipmentById(id);
        if (equipment == null)
        {
            _logger.LogWarning("Equipment {EquipmentId} was not found.", id);
            return NotFound();
        }

        return View(equipment);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Equipment equipment)
    {
        if (id != equipment.EquipmentId)
        {
            _logger.LogWarning("Equipment ID mismatch. Route ID: {RouteId}, Equipment ID: {EquipmentId}", id, equipment.EquipmentId);
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _equipmentRepository.Update(equipment);
                return RedirectToAction(nameof(Table));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while editing equipment {EquipmentId}.", equipment.EquipmentId);
                ModelState.AddModelError(string.Empty, "An error occurred while editing the equipment. Please try again later.");
            }
        }

        return View(equipment);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var equipment = await _equipmentRepository.GetEquipmentById(id);
        if (equipment == null)
        {
            _logger.LogWarning("Equipment {EquipmentId} was not found.", id);
            return NotFound();
        }

        return View(equipment);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var equipment = await _equipmentRepository.GetEquipmentById(id);
        if (equipment == null)
        {
             _logger.LogWarning("Equipment {EquipmentId} was not found.", id);
            return NotFound();
        }

        try
        {
            // Koblingene i RoomEquipment slettes automatisk (Cascade). Rommene blir stående.
            await _equipmentRepository.Delete(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting equipment {EquipmentId}.", id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the equipment. Please try again later.");
            return View("Delete", equipment);
        }
        return RedirectToAction(nameof(Table));
    }
}