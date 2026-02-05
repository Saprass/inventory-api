using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InventoryApiDb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

// Seed the in-memory database with initial mock data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!dbContext.Products.Any())
    {
        dbContext.Products.AddRange(
            new Product { Name = "Apples", Price = 1.20m, Stock = 100 },
            new Product { Name = "Bread", Price = 0.95m, Stock = 50},
            new Product { Name = "Fish", Price = 5.49m, Stock = 17 }
    );
        dbContext.SaveChanges();
    }
}

app.MapGet("/", () => "Hello World!");

app.Run();
