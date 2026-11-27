
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using thebooking.Models;

namespace thebooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildingAPIController : ControllerBase
{
    private readonly BookingDbContext _db;

    public BuildingAPIController(BookingDbContext db)
    {
        _db = db;
    }

    [HttpGet("buildinglist")]
    public async Task<IActionResult> BuildingList()
    {
        var buildings = await _db.Buildings
            .OrderBy(b => b.BuildingId)
            .Select(b => new
            {
                buildingId = b.BuildingId,
                name = b.Name
            })
            .ToListAsync();

        return Ok(buildings);
    }
}
