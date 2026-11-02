using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace thebooking.Models;

public class Building
{
    public int BuildingId { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "The building name must be between 2 and 50 characters.")]
    [Display(Name= "Building name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Address { get; set; }
    
    [ValidateNever]
    public virtual List<Room>? Rooms { get; set; }
}