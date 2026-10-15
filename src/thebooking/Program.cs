using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using thebooking.Models;
using Serilog;
using Serilog.Events;

var logFileName = $"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddSerilog((service, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(logFileName)
        .Filter.ByExcluding
        (e => e.Properties.TryGetValue("SourceContext", out var value) &&
              e.Level == LogEventLevel.Information &&
              e.MessageTemplate.Text.Contains("Executed DbCommand"));
});
builder.Services.AddDbContext<BookingDbContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:BookingDbContextConnection"]);
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // Disable email confirmation for development
}).AddEntityFrameworkStores<BookingDbContext>();
    
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
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
    db.Database.Migrate();
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapStaticAssets();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.Run();