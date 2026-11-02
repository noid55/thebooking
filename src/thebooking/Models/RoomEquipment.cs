using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace thebooking.Models;
public class RoomEquipment
{
    public int RoomEquipmentId{ get; set;}
    public int RoomId{get; set;}

    public int EquipmentId {get; set;}

    [ValidateNever]
    public virtual Room Room {get; set;} = default!;

    [ValidateNever]
    public virtual  Equipment Equipment {get; set; } = default!;


    
}