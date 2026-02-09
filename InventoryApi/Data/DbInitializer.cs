using InventoryApi.Models;

namespace InventoryApi.Data;

public class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Customers.Any())
        {
            return;
        }
        var customers = new Customer[] {
            new Customer { Name = "John Doe", Email = "john.doe@gmail.com", Address = "123 street", Phone = "+1 123-456-7890" },
            new Customer { Name = "Marijka Groen", Email = "marijka.groen@gmail.com", Address = "123 straat", Phone = "+31 123456789" }
        };

        context.Customers.AddRange(customers);
        context.SaveChanges();

        var products = new Product[] {
            new Product { Name = "Apples", Price = 1.20m, Stock = 100, IsActive = true },
            new Product { Name = "Bread", Price = 0.95m, Stock = 50, IsActive = true },
            new Product { Name = "Fish", Price = 5.49m, Stock = 17, IsActive = true }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
