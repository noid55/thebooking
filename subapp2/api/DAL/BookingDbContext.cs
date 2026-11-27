using Microsoft.EntityFrameworkCore;
using thebooking.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace thebooking.DAL;

public class BookingDbContext : IdentityDbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
       
    }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Building> Buildings{ get; set;}
    public DbSet<Equipment> Equipment{ get;set;}
    public DbSet<RoomEquipment> RoomEquipments {get;set;}
    public DbSet<Participation> Participations { get; set; }
    public DbSet<Message> Messages { get; set; }


   
 protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        base.OnModelCreating(modelBuilder);
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

        // samme utstyr kan bare kobles til samme rom en gang
        modelBuilder.Entity<RoomEquipment>()
            .HasIndex(re => new { re.RoomId, re.EquipmentId })
            .IsUnique();
        
        modelBuilder.Entity<Room>()
            .HasIndex(r => new { r.BuildingId, r.Name })
            .IsUnique();

        // A user who owns bookings cannot be deleted
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // When a booking is deleted, its participations are deleted too (Cascade)
        modelBuilder.Entity<Participation>()
            .HasOne(p => p.Booking)
            .WithMany(b => b.Participations)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Participation>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // A student can only join the same session once
        modelBuilder.Entity<Participation>()
            .HasIndex(p => new { p.BookingId, p.UserId })
            .IsUnique();

        // When a booking is deleted, its messages are deleted too (Cascade)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Booking)
            .WithMany(b => b.Messages)
            .HasForeignKey(m => m.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);


    }

}

