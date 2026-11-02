using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
namespace thebooking.Models;

public class Booking
{
    public int BookingId { get; set; }
    public int RoomId { get; set; }
    [ValidateNever]
    public virtual Room? Room { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "The topic must be between 2 and 100 characters.")]
    public string Topic { get; set;} = string.Empty;
  

    [Display(Name = "Start time")]
    public DateTime StartTime { get; set; }
    
    [Display(Name = "End time")]
    public DateTime EndTime { get; set; }

    public  DateTime CreatedAt { get; set; } = DateTime.Now;

    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

    /// The owner of the booking. Set by the server to the logged-in user's ID,
    // so a student cannot book in someone else's name.
    [BindNever]
    [ValidateNever]
    public string UserId { get; set; } = string.Empty;


}
