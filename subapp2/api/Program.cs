using thebooking.DAL;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using thebooking.Services;
using Serilog;
using Serilog.Events;

// Every run of the app gets its own log file, with date and time in the name
var logFileName = $"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddSerilog((service, loggerConfiguration) =>
{
    // Log messages are written both to the terminal and to the log file in the Logs folder.
    // The filter removes the "Executed DbCommand" messages. EF Core writes one for every
    // SQL query, so without the filter the log would fill up.
    loggerConfiguration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(logFileName)
        .Filter.ByExcluding
        (e => e.Properties.TryGetValue("SourceContext", out var value) &&
              e.Level == LogEventLevel.Information &&
              e.MessageTemplate.Text.Contains("Executed DbCommand"));
});

builder.Services.AddOpenApi();
builder.Services.AddDbContext<BookingDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("BookingDbContextConnection")));

// Repository pattern (demo 6): connects each interface to the class that should be used (dependency injection).
// Scoped = one instance per HTTP request, the same lifetime as the DbContext.
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IParticipationRepository, ParticipationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
// Service layer with the domain rules for booking.
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