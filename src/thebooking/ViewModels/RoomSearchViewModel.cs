using Microsoft.AspNetCore.Mvc.Rendering;
using thebooking.Models;

namespace thebooking.ViewModels;

// The rooms to show on the Rooms page, plus the current search values and the lists for the search form
public class RoomSearchViewModel
{
    public IEnumerable<Room> Rooms { get; set; } = new List<Room>();

    // Search values (null or empty = not used)
    public int? BuildingId { get; set; }
    public int? MinCapacity { get; set; }
    public List<int> EquipmentIds { get; set; } = new();

    public List<SelectListItem> BuildingSelectList { get; set; } = new();
    public List<SelectListItem> EquipmentSelectList { get; set; } = new();
}