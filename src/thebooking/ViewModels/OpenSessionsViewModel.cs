using Microsoft.AspNetCore.Mvc.Rendering;
using thebooking.Models;

namespace thebooking.ViewModels;

// The open study sessions on the "Find study groups" page, plus the current search values
public class OpenSessionsViewModel
{
    public IEnumerable<Booking> Sessions { get; set; } = new List<Booking>();

    // Search values (null or empty = not used)
    public string? Course { get; set; }
    public int? BuildingId { get; set; }
    public DateTime? Date { get; set; }

    public List<SelectListItem> BuildingSelectList { get; set; } = new();
}