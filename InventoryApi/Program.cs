using Microsoft.EntityFrameworkCore;
using InventoryApi.Data;
using InventoryApi.DTOs.Orders;
using InventoryApi.Services;
using InventoryApi.Mappings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config => 
{
    config.DocumentName = "InventoryAPI";
    config.Title = "Inventory API";
    config.Version = "v1";
});

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config => 
    {
        config.DocumentTitle = "Inventory API Documentation";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

CreateDbIfNotExists(app);

app.MapControllers();

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
    ServiceResult<OrderDetailDTO> result = await orderService.CreateOrderAsync(createOrder);

    return ResultHttpExtensions.ToHttp(result);
});

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
