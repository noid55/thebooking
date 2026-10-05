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
app.MapDefaultControllerRoute();
app.Run();
