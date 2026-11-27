using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace thebooking.Models;

public class BookingDbContext : IdentityDbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Building> Buildings { get; set; }
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<RoomEquipment> RoomEquipments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Booking til Room
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Room til Building
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Building)
            .WithMany(b => b.Rooms)
            .HasForeignKey(r => r.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Et bygg kan ikke ha to rom med samme navn
        modelBuilder.Entity<Room>()
            .HasIndex(r => new { r.BuildingId, r.Name })
            .IsUnique();

        // Samme utstyr kan ikke registreres flere ganger på samme rom
        modelBuilder.Entity<RoomEquipment>()
            .HasIndex(re => new { re.RoomId, re.EquipmentId })
            .IsUnique();
    }
}