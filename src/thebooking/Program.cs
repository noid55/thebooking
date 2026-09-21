using Microsoft.EntityFrameworkCore;
using thebooking.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BookingDbContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:BookingDbContextConnection"]);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapDefaultControllerRoute();
app.Run();