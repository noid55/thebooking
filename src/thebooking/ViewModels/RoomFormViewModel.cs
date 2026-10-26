using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using thebooking.Models;

namespace thebooking.ViewModels;

public class RoomFormViewModel
{
    public Room Room { get; set; } = default!;
    [ValidateNever]
    public List<SelectListItem> BuildingSelectList { get; set; } = default!;
    [ValidateNever]
    public List<SelectListItem> EquipmentSelectList { get; set; } = default!;
}