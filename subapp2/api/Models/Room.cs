using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace thebooking.Models;

public class Room
{
    public int RoomId { get; set; }

    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,20}", ErrorMessage = "The name must consist of numbers and letters and be between 2 and 20 characters long.")]
    [Display(Name = "Room Name")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "The capacity must be between 1 and 20.")]
    public int Capacity { get; set; }
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    //validering av byggnavn ligger nå i Building.Name
    [Display(Name = "Building")]
    public int BuildingId { get; set; }
    [ValidateNever]
    public virtual Building? Building { get; set;}

    [ValidateNever]
    public virtual List<RoomEquipment>? RoomEquipments { get; set; }
    
    [ValidateNever]
    public virtual List<Booking>? Bookings { get; set; }

}