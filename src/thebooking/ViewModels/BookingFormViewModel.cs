using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using thebooking.Models;

namespace thebooking.ViewModels;

public class BookingFormViewModel
{
    public Booking Booking { get; set; } = default!;
    [ValidateNever]
    public List<SelectListItem> RoomSelectList { get; set; } = default!;
}