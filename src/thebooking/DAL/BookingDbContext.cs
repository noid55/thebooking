using Microsoft.EntityFrameworkCore;
using thebooking.Models;

namespace thebooking.DAL;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
       
    }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Building> Buildings{ get; set;}
    public DbSet<Equipment> Equipment{ get;set;}
    public DbSet<RoomEquipment> RoomEquipments {get;set;}


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
 protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        //et rom som har bookinger kan ikke slettes
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
            
        //et bygg som har rom kan ikke slettes
        modelBuilder.Entity<Room>()
        .HasOne(r => r.Building)
        .WithMany(b => b.Rooms)
        .HasForeignKey(r => r.BuildingId)
        .OnDelete(DeleteBehavior.Restrict);

        //et rom som har bookinger kan ikke slettes
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // samme utstyr kan bare kobles til samme rom en gang
        modelBuilder.Entity<RoomEquipment>()
            .HasIndex(re => new { re.RoomId, re.EquipmentId })
            .IsUnique();
        
        // et bygg kan ikke ha to rom med samme navn
        modelBuilder.Entity<Room>()
            .HasIndex(r => new { r.BuildingId, r.Name })
            .IsUnique();


    }

}

