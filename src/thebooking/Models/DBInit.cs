using Microsoft.EntityFrameworkCore;

namespace thebooking.Models;

public static class DBInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        BookingDbContext context = serviceScope.ServiceProvider.GetRequiredService<BookingDbContext>();

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

        if (!context.Bookings.Any())
        {
            var tomorrow = DateTime.Today.AddDays(1);
            var bookings = new List<Booking>
            {
                new Booking { RoomId = 1, Topic = "Study group DATA1500", UserId = "seed-user-1",
                              StartTime = tomorrow.AddHours(10), EndTime = tomorrow.AddHours(12) },
                new Booking { RoomId = 1, Topic = "Exam preparation",     UserId = "seed-user-2",
                              StartTime = tomorrow.AddHours(12), EndTime = tomorrow.AddHours(14) },
                new Booking { RoomId = 3, Topic = "Project meeting",      UserId = "seed-user-1",
                              StartTime = tomorrow.AddHours(9),  EndTime = tomorrow.AddHours(11) },
                new Booking { RoomId = 5, Topic = "Group presentation",   UserId = "seed-user-3",
                              StartTime = tomorrow.AddDays(1).AddHours(13), EndTime = tomorrow.AddDays(1).AddHours(15) },
                new Booking { RoomId = 2, Topic = "Cancelled meeting",    UserId = "seed-user-2",
                              StartTime = tomorrow.AddHours(14), EndTime = tomorrow.AddHours(16),
                              Status = BookingStatus.Cancelled },
            };
            context.AddRange(bookings);
            context.SaveChanges();
        }
    }
}