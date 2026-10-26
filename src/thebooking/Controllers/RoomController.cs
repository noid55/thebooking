using Microsoft.AspNetCore.Mvc;
using thebooking.Models;
using thebooking.DAL;
using thebooking.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace thebooking.Controllers;

public class RoomController : Controller
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly ILogger<RoomController> _logger;

    public RoomController(IRoomRepository roomRepository, IBuildingRepository buildingRepository,
        IEquipmentRepository equipmentRepository, ILogger<RoomController> logger)
    {
        _roomRepository = roomRepository;
        _buildingRepository = buildingRepository;
        _equipmentRepository = equipmentRepository;
        _logger = logger;
    }

    // demo 5: lager ViewModel-en med rommet + listene over bygg og utstyr
    private async Task<RoomFormViewModel> CreateRoomFormViewModel(Room room, List<int> selectedEquipmentIds)
    {
        var buildings = await _buildingRepository.GetAll();
        var equipment = await _equipmentRepository.GetAll();
        return new RoomFormViewModel
        {
            Room = room,
            BuildingSelectList = buildings.Select(building => new SelectListItem
            {
                Value = building.BuildingId.ToString(),
                Text = building.Name
            }).ToList(),
            EquipmentSelectList = equipment.Select(e => new SelectListItem
            {
                Value = e.EquipmentId.ToString(),
                Text = e.Name,
                Selected = selectedEquipmentIds.Contains(e.EquipmentId)
            }).ToList()
        };
    }

    public async Task<IActionResult> Table()
    {
        _logger.LogInformation("The study room table was accessed.");

        var rooms = await _roomRepository.GetAll();
        ViewBag.CurrentViewName = "List of Rooms";
        return View(rooms);
    }

    public async Task<IActionResult> Details(int id)
    {
        var room = await _roomRepository.GetRoomById(id);
        if (room == null)
        {
            _logger.LogWarning("Room {RoomId} was not found.", id);
            return NotFound();
        }

        return View(room);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await CreateRoomFormViewModel(new Room(), new List<int>()));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Room room, List<int> selectedEquipmentIds)
    {
        if (ModelState.IsValid && await _roomRepository.NameExistsInBuilding(room.Name, room.BuildingId))
        {
            ModelState.AddModelError("Room.Name", "A room with this name already exists in this building.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _roomRepository.Create(room);
                 await _roomRepository.UpdateEquipment(room.RoomId, selectedEquipmentIds); //slik at utstyret lagres
                return RedirectToAction(nameof(Table));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating room {RoomName}.", room.Name);
                ModelState.AddModelError(string.Empty, "An error occurred while creating the room. Please try again later.");
            }
        }

        return View(await CreateRoomFormViewModel(room, selectedEquipmentIds));
    }    

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var room = await _roomRepository.GetRoomById(id);
        if (room == null)
        {
            _logger.LogWarning("Room {RoomId} was not found.", id);
            return NotFound();
        }

        // utstyret rommet har fra før skal være krysset av
        var currentEquipmentIds = room.RoomEquipments?.Select(re => re.EquipmentId).ToList() ?? new List<int>();
        return View(await CreateRoomFormViewModel(room, currentEquipmentIds));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Room room, List<int> selectedEquipmentIds)
    {
        if (id != room.RoomId)
        {
            _logger.LogWarning("Room ID mismatch. Route ID: {RouteId}, Room ID: {RoomId}", id, room.RoomId);
            return NotFound();
        }

        if (ModelState.IsValid && await _roomRepository.NameExistsInBuilding(room.Name, room.BuildingId, room.RoomId))
        {
            ModelState.AddModelError("Room.Name", "A room with this name already exists in this building.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _roomRepository.Update(room);
                await _roomRepository.UpdateEquipment(room.RoomId, selectedEquipmentIds);
                return RedirectToAction(nameof(Table));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while editing room {RoomId}.", room.RoomId);
                ModelState.AddModelError(string.Empty, "An error occurred while editing the room. Please try again later.");
            }
        }

        return View(await CreateRoomFormViewModel(room, selectedEquipmentIds));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _roomRepository.GetRoomById(id);
        if (room == null)
        {
            _logger.LogWarning("Room {RoomId} was not found.", id);
            return NotFound();
        }

        return View(room);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var room = await _roomRepository.GetRoomById(id);
        if (room == null)
        {
            _logger.LogWarning("Room {RoomId} was not found.", id);
            return NotFound();
        }

        try
        {
            await _roomRepository.Delete(id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while deleting room {RoomId}.", id);
            ModelState.AddModelError(string.Empty, "The room could not be deleted. It may have bookings.");
            return View("Delete", room);
        }
        return RedirectToAction(nameof(Table));
    }
}