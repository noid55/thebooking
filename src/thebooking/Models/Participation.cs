using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace thebooking.Models;

// A student who has joined an open study session (Booking).
// A student can only join the same session once (unique index in BookingDbContext).
public class Participation
{
    public int ParticipationId { get; set; }

    public int BookingId { get; set; }

    [ValidateNever]
    public virtual Booking? Booking { get; set; }

    // Set by the server to the logged-in user's ID
    public string UserId { get; set; } = string.Empty;

    [ValidateNever]
    public virtual IdentityUser? User { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.Now;
}