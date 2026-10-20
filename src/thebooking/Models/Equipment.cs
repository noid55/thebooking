using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace thebooking.Models;
public class Equipment
{
    public int EquipmentId { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "The equipment name must be between 2 and 50 characters.")]
    public string Name {get; set;} = string.Empty;

    [ValidateNever]
    public virtual List<RoomEquipment>? RoomEquipments { get; set;}
}