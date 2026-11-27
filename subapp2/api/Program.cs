using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using thebooking.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<BookingDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("BookingDbContextConnection")));

builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", p =>
    p.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
     .AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("CorsPolicy");
app.MapControllers();
app.Run();
