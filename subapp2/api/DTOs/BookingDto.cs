namespace thebooking.DTOs;

public class BookingDto
{
    public int BookingId { get; set; }
    public int RoomId { get; set; }
    public string? RoomName { get; set; }   // out only: so React can show the room
    public string Course { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsOpen { get; set; } = true;
    public int MaxParticipants { get; set; } = 4;
    public string? Status { get; set; }     // out only: "Confirmed" or "Cancelled"
}