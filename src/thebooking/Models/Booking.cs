using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
namespace thebooking.Models;

// A booking is a study session in a room.
// The student who books the room is the owner. If the session is open, other students can join.
public class Booking
{
    public int BookingId { get; set; }

    [Display(Name = "Room")]
    public int RoomId { get; set; }
    [ValidateNever]
    public virtual Room? Room { get; set; }

    // Course code, used when students search for open study groups (e.g. "DATA1500")
    [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ \-]{2,20}", ErrorMessage = "The course must be 2-20 letters or numbers, for example DATA1500.")]
    public string Course { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 2, ErrorMessage = "The topic must be between 2 and 100 characters.")]
    public string Topic { get; set;} = string.Empty;
  

    [Display(Name = "Start time")]
    public DateTime StartTime { get; set; }
    
    [Display(Name = "End time")]
    public DateTime EndTime { get; set; }

    public  DateTime CreatedAt { get; set; } = DateTime.Now;

    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

    // true = other students can find and join the session. false = private booking.
    [Display(Name = "Open for other students to join")]
    public bool IsOpen { get; set; } = true;

    // Total number of people in the session, including the owner.
    // BookingService checks that this is not higher than the room's capacity.
    [Range(1, 20, ErrorMessage = "Max participants must be between 1 and 20.")]
    [Display(Name = "Max participants")]
    public int MaxParticipants { get; set; } = 4;

    // The owner of the booking. Set by the server to the logged-in user's ID,
    // so a student cannot book in someone else's name.
    [BindNever]
    [ValidateNever]
    public string UserId { get; set; } = string.Empty;

    // Navigation to the user who owns the booking. UserId is a foreign key to AspNetUsers.
    [ValidateNever]
    public virtual IdentityUser? User { get; set; }

    // Students who have joined the session (the owner is not in this list)
    [ValidateNever]
    public virtual List<Participation>? Participations { get; set; }

    // Messages left in the session
    [ValidateNever]
    public virtual List<Message>? Messages { get; set; }
}