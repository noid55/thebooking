using Microsoft.EntityFrameworkCore;

namespace thebooking.Models;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<Room> Rooms { get; set; }
}