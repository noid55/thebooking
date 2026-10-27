using Microsoft.EntityFrameworkCore;
using thebooking.DAL;
using Serilog;
using Serilog.Events;
using thebooking.Services;

// Hver kjøring av appen får sin egen loggfil, med dato og klokkeslett i navnet
var logFileName = $"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log";

var builder = WebApplication.CreateBuilder(args);

//controllere med Razor-views
builder.Services.AddControllersWithViews();

builder.Services.AddSerilog((service, loggerConfiguration) =>
{
    //Loggmeldinger skrives både til terminalen og til loggfilen i Logs-mappen.
    // Bare meldinger som Warning og Error lagres. Mens Debug-meldinger hoppes over.
    // Filteret fjerner "Executed DbCommand"-meldingene. EF Core skriver en slik melding for hver
    // SQL-spørring (slik at loggen ikke blir fylt opp)
    loggerConfiguration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(logFileName)
        .Filter.ByExcluding
        (e => e.Properties.TryGetValue("SourceContext", out var value) &&
              e.Level == LogEventLevel.Information &&
              e.MessageTemplate.Text.Contains("Executed DbCommand"));
});
//BookingDbContext med SQLite.
builder.Services.AddDbContext<BookingDbContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:BookingDbContextConnection"]);
});

// Repository (demo 6): kobler hvert interface til klassen som skal brukes (dependency injection).
// Scoped = én instans per HTTP-forespørsel, samme levetid som DbContext-en.
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
//Service layer med domenereglene for booking
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    db.Database.Migrate();
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    DBInit.Seed(app);
}

app.MapStaticAssets();
app.MapDefaultControllerRoute();
app.Run();
