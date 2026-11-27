using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using thebooking.Models;

namespace thebooking.ViewModels;

// Everything the Booking Details page needs: the booking, what the logged-in user is allowed to do,
// and an empty message for the message form
public class BookingDetailsViewModel
{
    public Booking Booking { get; set; } = default!;

    public string CurrentUserId { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }

    public bool CanManage { get; set; }      // Owner or admin: edit, cancel, delete
    public bool IsParticipant { get; set; }  // Has joined the session: can leave
    public bool CanJoin { get; set; }        // All join rules in BookingService are OK
    public bool CanPostMessage { get; set; } // Owner or participant

    public bool CanSeeMessages { get; set; } // Owner, participants and admin (messages are private to the session)

    public List<string> JoinErrors { get; set; } = new(); // Why the user cannot join (shown on the page)

    [ValidateNever]
    public Message NewMessage { get; set; } = new();
}