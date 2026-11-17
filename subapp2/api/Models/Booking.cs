namespace thebooking.Models;

public class Booking
{
    public int BookingId { get; set; }
    public int RoomId { get; set; }
    public virtual Room Room { get; set; } = default!;
    public string Topic { get; set;} = string.Empty;
  

    public string BookedBy { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public  DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
