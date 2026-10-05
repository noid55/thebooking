using System;
using System.ComponentModel.DataAnnotations;
namespace thebooking.Models;

public class Room
{
    public int RoomId { get; set; }

    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,20}", ErrorMessage = "The name must be consist of numbers and letter and be between 2 and 20 characters long.")]
    [Display(Name = "Room Name")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "The capacity must be between 1 and 20.")]
    public int Capacity { get; set; }

    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,20}", ErrorMessage = "The building name must be consist of numbers and letter and be between 2 and 20 characters long.")]
    [Display(Name = "Building")]
    public string Building { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

      public virtual List<Booking>? Bookings { get; set; }

}