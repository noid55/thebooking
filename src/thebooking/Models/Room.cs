using System;
namespace thebooking.Models;

public class Room
{
    public int RoomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Building { get; set; } = string.Empty;
    public string? Description { get; set; }

}