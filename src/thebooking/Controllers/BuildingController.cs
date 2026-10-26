using Microsoft.AspNetCore.Mvc;
using thebooking.DAL;
using thebooking.Models;

namespace thebooking.Controllers;

public class BuildingController : Controller
{
    private readonly IBuildingRepository _buildingRepository;
    private readonly ILogger<BuildingController> _logger;

    public BuildingController(IBuildingRepository buildingRepository, ILogger<BuildingController> logger)
    {
        _buildingRepository = buildingRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Table()
    {
        _logger.LogInformation("The building table was accessed.");

        var buildings = await _buildingRepository.GetAll();
        ViewBag.CurrentViewName = "List of Buildings";
        return View(buildings);
    }

    public async Task<IActionResult> Details(int id)
    {
        var building = await _buildingRepository.GetBuildingById(id);
        if (building == null)
        {
            _logger.LogWarning("Building {BuildingId} was not found.", id);
            return NotFound();
        }

        return View(building);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Building building)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _buildingRepository.Create(building);
                return RedirectToAction(nameof(Table));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating building {BuildingName}.", building.Name);
                ModelState.AddModelError(string.Empty, "An error occurred while creating the building. Please try again later.");
            }
        }

        return View(building);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var building = await _buildingRepository.GetBuildingById(id);
        if (building == null)
        {
            _logger.LogWarning("Building {BuildingId} was not found.", id);
            return NotFound();
        }

        return View(building);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Building building)
    {
        if (id != building.BuildingId)
        {
            _logger.LogWarning("Building ID mismatch. Route ID: {RouteId}, Building ID: {BuildingId}", id, building.BuildingId);
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _buildingRepository.Update(building);
                return RedirectToAction(nameof(Table));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while editing building {BuildingId}.", building.BuildingId);
                ModelState.AddModelError(string.Empty, "An error occurred while editing the building. Please try again later.");
            }
        }

        return View(building);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var building = await _buildingRepository.GetBuildingById(id);
        if (building == null)
        {
            _logger.LogWarning("Building {BuildingId} was not found.", id);
            return NotFound();
        }

        return View(building);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var building = await _buildingRepository.GetBuildingById(id);
        if (building == null)
        {
            _logger.LogWarning("Building {BuildingId} was not found.", id);
            return NotFound();
        }

        try
        {
            await _buildingRepository.Delete(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting building {BuildingId}.", id);
            ModelState.AddModelError(string.Empty, "The building could not be deleted. It may still have rooms.");
            return View("Delete", building);
        }
        return RedirectToAction(nameof(Table));
    }
}