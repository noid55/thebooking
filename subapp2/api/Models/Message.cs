using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace thebooking.Models;

// A message left in a study session by the owner or a participant.
public class Message
{
    public int MessageId { get; set; }

    public int BookingId { get; set; }

    [ValidateNever]
    public virtual Booking? Booking { get; set; }

    // Set by the server to the logged-in user's ID, so no one can post in someone else's name
    [BindNever]
    [ValidateNever]
    public string UserId { get; set; } = string.Empty;

    [ValidateNever]
    public virtual IdentityUser? User { get; set; }

    [StringLength(500, MinimumLength = 1, ErrorMessage = "The message must be between 1 and 500 characters.")]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}