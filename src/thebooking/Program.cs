using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using thebooking.DAL;
using Serilog;
using Serilog.Events;
using thebooking.Services;
using thebooking.Data;

// Every run of the app gets its own log file, with date and time in the name
var logFileName = $"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log";

var builder = WebApplication.CreateBuilder(args);

// Controllers with Razor views
builder.Services.AddControllersWithViews();

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
// BookingDbContext with SQLite
builder.Services.AddDbContext<BookingDbContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:BookingDbContextConnection"]);
});

// Repository pattern (demo 6): connects each interface to the class that should be used (dependency injection).
// Scoped = one instance per HTTP request, the same lifetime as the DbContext.
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBuildingRepository, BuildingRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IParticipationRepository, ParticipationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
// Service layer with the domain rules for booking
builder.Services.AddScoped<IBookingService, BookingService>();


// Identity with roles, so [Authorize(Roles = "Admin")] can be used
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // Disable email confirmation for development
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<BookingDbContext>();
    
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

    // Creates/updates the database from the migrations
    db.Database.Migrate();

    // Roles (Admin, Manager, User) and the admin account from appsettings.json
    await RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
    await AdminSeeder.SeedAdminAsync(scope.ServiceProvider);
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    await DBInit.Seed(app); // Test data: students, rooms and study sessions
}

app.MapStaticAssets();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.Run();