using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.Models;
using InventoryApi.DTOs.Products;
using InventoryApi.DTOs.Customers;
using InventoryApi.DTOs.Orders;
using InventoryApi.DTOs.Common;
using InventoryApi.Services;
using InventoryApi.Mappings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

CreateDbIfNotExists(app);

app.MapGet("/products", async (AppDbContext db) =>
    Results.Ok(await db.Products.Select(x => new ProductDTO(x)).ToListAsync()));

app.MapGet("/products/{id:int:min(1)}", async (int id, AppDbContext db) =>
    await db.Products.FindAsync(id)
        is Product product
            ? Results.Ok(new ProductDTO(product))
            : Results.NotFound());

app.MapPost("/products", async (ProductCreateDTO createProduct, IProductService productService, AppDbContext db) =>
{
    ServiceResult<int> result = await productService.CreateProductAsync(createProduct);

    return await ResultHttpExtensions.ToHttpAsync(result, async productId => {
        var dtoResp = await db.Products
            .Where(p => p.Id == productId)
            .Select(p => new ProductDTO(p))
            .FirstAsync();
        return Results.Created($"/products/{productId}", dtoResp);
    });
});

app.MapPatch("/products/{id:int:min(1)}", async (int id, ProductPatchDTO patchedProduct, IProductService productService, AppDbContext db) =>
{
    ServiceResult result = await productService.UpdateProductAsync(id, patchedProduct);
    
    return ResultHttpExtensions.ToHttp(result);
});

app.MapPatch("/products/{id:int:min(1)}/deactivate", async (int id, IProductService productService, AppDbContext db) =>
{
    ServiceResult result = await productService.DeactivateProductAsync(id, false);

    return ResultHttpExtensions.ToHttp(result);
});

app.MapGet("/customers", async (AppDbContext db) =>
    Results.Ok((await db.Customers.Select(x => new CustomerDTO(x)).ToListAsync())));

app.MapGet("/customers/{id:int:min(1)}", async (int id, AppDbContext db) =>
    await db.Customers.FindAsync(id)
        is Customer customer
            ? Results.Ok(new CustomerDTO(customer))
            : Results.NotFound());

app.MapPost("/customers", async (CustomerCreateDTO createCustomer, ICustomerService customerService, AppDbContext db) =>
{
    ServiceResult<int> result = await customerService.CreateCustomerAsync(createCustomer);
    int customerId = result.Value;

    CustomerDTO dtoResp = await db.Customers
        .Where(c => c.Id == customerId)
        .Select(c => new CustomerDTO(c))
        .FirstAsync();

    return Results.Created($"/customers/{customerId}", dtoResp);
});

app.MapPatch("/customers/{id:int:min(1)}", async (int id, CustomerPatchDTO patchedCustomer, ICustomerService customerService, AppDbContext db) =>
{
    ServiceResult result = await customerService.UpdateCustomerAsync(id, patchedCustomer);

    return ResultHttpExtensions.ToHttp(result);
});

app.MapGet("/orders", async (AppDbContext db) => 
{
    var orders = await db.Orders
        .SelectOrderSummary()
        .ToListAsync();
    
    return Results.Ok(orders);
});

app.MapGet("/orders/{id:int:min(1)}", async (int id, AppDbContext db) => 
{
    var orderDto = await db.Orders
        .SelectOrderDetail(id)
        .FirstOrDefaultAsync();

    return orderDto is null ? Results.NotFound() : Results.Ok(orderDto);
});

app.MapPatch("/orders/{id:int:min(1)}/status", async (int id, OrderStatusUpdateDTO oStatusDTO, IOrderService orderService, AppDbContext db) => 
{
    ServiceResult result = await orderService.UpdateOrderStatusAsync(id, oStatusDTO.Status);

    return ResultHttpExtensions.ToHttp(result);
});

app.MapPost("/orders", async (OrderCreateDTO createOrder, IOrderService orderService, AppDbContext db) =>
{
    ServiceResult<int> result = await orderService.CreateOrderAsync(createOrder);
    
    int orderId = result.Value;
    return await ResultHttpExtensions.ToHttpAsync(result, async orderId => {
        var dtoResp = await db.Orders
            .Where(o => o.Id == orderId)
            .Select(o => new OrderDetailDTO(
                o.Id,
                new CustomerInfoDTO(o.Customer.Id, o.Customer.Name),
                o.OrderDate,
                o.OrderStatus.ToString(),
                o.TotalAmount,
                o.OrderItems.Select(oi => new OrderItemDTO(
                    oi.Id,
                    new ProductInfoDTO(oi.ProductId, oi.Product.Name),
                    oi.Quantity,
                    oi.UnitPrice
                )).ToList()
            )).FirstOrDefaultAsync();

        return Results.Created($"/orders/{orderId}", dtoResp);
    });
});

app.MapGet("/", () => "Hello World!");

app.Run();

static void CreateDbIfNotExists(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            DbInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred creating the DB.");
        }
    }
}
