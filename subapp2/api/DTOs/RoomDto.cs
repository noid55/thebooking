namespace thebooking.DTOs;

public class RoomDto
{
    public int RoomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    
    public int BuildingId { get; set; }        // in and out: which building the room belongs to
    public string? BuildingName { get; set; }  // out only: so React can display the name
    public string? Description { get; set; }
}
