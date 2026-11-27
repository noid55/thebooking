using thebooking.DAL;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using thebooking.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<BookingDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("BookingDbContextConnection")));

// Repository (demo 6): kobler hvert interface til klassen som skal brukes (dependency injection).
// Scoped = én instans per HTTP-forespørsel, samme levetid som DbContext-en.
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IParticipationRepository, ParticipationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
//Service layer med domenereglene for booking
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", p =>
    p.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
        .AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    db.Database.Migrate();
    
}
DBInit.Seed(app);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("CorsPolicy");
app.MapControllers();
app.Run();