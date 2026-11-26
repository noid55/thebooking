using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using thebooking.Models;

namespace thebooking.DAL;

// Fills an empty database with test data (only runs in Development, see Program.cs).
// Roles and the admin are created by RoleSeeder and AdminSeeder in the Data folder, which run before this.
// Test users (also listed in the documentation):
//   admin@oslomet.no    / Admin123!    (role: Admin, created by AdminSeeder)
//   student1@oslomet.no / Student123!  (role: User)
//   student2@oslomet.no / Student123!  (role: User)
public static class DBInit
{
    // Async, because UserManager only has async methods
    public static async Task Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        BookingDbContext context = serviceScope.ServiceProvider.GetRequiredService<BookingDbContext>();
        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // The students must exist before the bookings, because Booking.UserId is a foreign key
        var student1 = await CreateStudentIfMissing(userManager, "student1@oslomet.no", "Student123!");
        var student2 = await CreateStudentIfMissing(userManager, "student2@oslomet.no", "Student123!");

        if (!context.Buildings.Any())
        {
            var buildings = new List<Building>
            {
                new Building { Name = "P32", Address = "Pilestredet 32" },
                new Building { Name = "P35", Address = "Pilestredet 35" },
                new Building { Name = "P46", Address = "Pilestredet 46" },
                new Building { Name = "P48", Address = "Pilestredet 48" },
                new Building { Name = "P52", Address = "Pilestredet 52" },
            };
            context.AddRange(buildings);
            context.SaveChanges();
        }

        if (!context.Equipment.Any())
        {
            var equipment = new List<Equipment>
            {
                new Equipment { Name = "Projector" },
                new Equipment { Name = "Whiteboard" },
                new Equipment { Name = "Screen" },
                new Equipment { Name = "Video conference" },
            };
            context.AddRange(equipment);
            context.SaveChanges();
        }

        if (!context.Rooms.Any())
        {
            var rooms = new List<Room>
            {
                new Room { Name = "Grupperom 101", Capacity = 4,  Description = "Small quiet room",      BuildingId = 1 },
                new Room { Name = "Grupperom 102", Capacity = 6,  Description = "Room with large table", BuildingId = 1 },
                new Room { Name = "Grupperom 201", Capacity = 8,  Description = "Bright room with view", BuildingId = 2 },
                new Room { Name = "Grupperom 202", Capacity = 2,  Description = "Room for two",          BuildingId = 2 },
                new Room { Name = "Grupperom 301", Capacity = 10, Description = "Room for larger groups", BuildingId = 3 },
                new Room { Name = "Grupperom 401", Capacity = 6,  Description = "Close to the cafeteria", BuildingId = 4 },
                new Room { Name = "Grupperom 501", Capacity = 12, Description = "Seminar style room",    BuildingId = 5 },
            };
            context.AddRange(rooms);
            context.SaveChanges();
        }

        if (!context.RoomEquipments.Any())
        {
            var roomEquipments = new List<RoomEquipment>
            {
                new RoomEquipment { RoomId = 1, EquipmentId = 2 }, // 101: Whiteboard
                new RoomEquipment { RoomId = 2, EquipmentId = 1 }, // 102: Projector
                new RoomEquipment { RoomId = 2, EquipmentId = 2 }, // 102: Whiteboard
                new RoomEquipment { RoomId = 3, EquipmentId = 3 }, // 201: Screen
                new RoomEquipment { RoomId = 5, EquipmentId = 1 }, // 301: Projector
                new RoomEquipment { RoomId = 5, EquipmentId = 2 }, // 301: Whiteboard
                new RoomEquipment { RoomId = 5, EquipmentId = 4 }, // 301: Video conference
                new RoomEquipment { RoomId = 6, EquipmentId = 3 }, // 401: Screen
                new RoomEquipment { RoomId = 7, EquipmentId = 1 }, // 501: Projector
                new RoomEquipment { RoomId = 7, EquipmentId = 4 }, // 501: Video conference
            };
            context.AddRange(roomEquipments);
            context.SaveChanges();
        }

        // Study sessions owned by the test students
        if (!context.Bookings.Any())
        {
            var tomorrow = DateTime.Today.AddDays(1);
            var bookings = new List<Booking>
            {
                new Booking { RoomId = 1, Course = "DATA1500", Topic = "Study group SQL joins", UserId = student1.Id,
                              IsOpen = true, MaxParticipants = 4,
                              StartTime = tomorrow.AddHours(10), EndTime = tomorrow.AddHours(12) },
                new Booking { RoomId = 3, Course = "ITPE3200", Topic = "Exam preparation",      UserId = student2.Id,
                              IsOpen = true, MaxParticipants = 6,
                              StartTime = tomorrow.AddHours(9),  EndTime = tomorrow.AddHours(11) },
                new Booking { RoomId = 5, Course = "DAFE2200", Topic = "Project meeting",       UserId = student1.Id,
                              IsOpen = false, MaxParticipants = 5,
                              StartTime = tomorrow.AddDays(1).AddHours(13), EndTime = tomorrow.AddDays(1).AddHours(15) },
                new Booking { RoomId = 2, Course = "DATA1700", Topic = "Cancelled meeting",     UserId = student2.Id,
                              IsOpen = true, MaxParticipants = 4,
                              StartTime = tomorrow.AddHours(14), EndTime = tomorrow.AddHours(16),
                              Status = BookingStatus.Cancelled },
            };
            context.AddRange(bookings);
            context.SaveChanges();

            // student2 has joined student1's open DATA1500 session and left a message
            var dataSession = bookings[0];
            context.Add(new Participation { BookingId = dataSession.BookingId, UserId = student2.Id });
            context.Add(new Message
            {
                BookingId = dataSession.BookingId,
                UserId = student2.Id,
                Text = "Hi! I will bring the exercises from week 5."
            });
            context.SaveChanges();
        }
    }

    // Creates a student if it does not exist yet, and gives it the "User" role
    // (the same role AccountController.Register gives new users)
    private static async Task<IdentityUser> CreateStudentIfMissing(UserManager<IdentityUser> userManager, string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                // Stops the startup with a clear error if the password does not follow Identity's rules
                throw new InvalidOperationException(
                    $"Could not create seed user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, "User"))
        {
            await userManager.AddToRoleAsync(user, "User");
        }
        return user;
    }
}