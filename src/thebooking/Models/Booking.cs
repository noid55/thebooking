using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
  

    public string UserId { get; set; } = string.Empty; //Byttet BookedBy med UserId 

    [Display(Name = "Start time")]
    public DateTime StartTime { get; set; }
    
    [Display(Name = "End time")]
    public DateTime EndTime { get; set; }

    public  DateTime CreatedAt { get; set; } = DateTime.Now;

    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;


}
