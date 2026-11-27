namespace thebooking.DTOs;

public class RoomDto
{
    public int RoomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int BuildingId { get; set; }
    public string BuildingName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
