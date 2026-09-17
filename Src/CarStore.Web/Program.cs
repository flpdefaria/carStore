using CarStore.Application.Services;
using CarStore.Domain.Data;
using CarStore.Domain.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CarStoreContext>(options =>
    options.UseInMemoryDatabase("CarStoreDb"));

builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CarStoreContext>();
    DataSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Any route not matched by an API/MVC endpoint (e.g. /cars, /brands/123) is a Vue Router route.
app.MapFallbackToController("Index", "Home");

app.Run();
