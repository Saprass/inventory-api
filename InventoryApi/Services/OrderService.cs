using InventoryApi.Data;
using InventoryApi.DTOs.Orders;
using InventoryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db) => _db = db;

    public async Task<ServiceResult<int>> CreateOrderAsync(OrderCreateDTO dto)
    {
        bool ok;
        ServiceResult<int>? result = null;

        (ok, result) = ValidateCreateRequest(dto);
        if (!ok)
            return (ServiceResult<int>)result!;

        var customer = await _db.Customers.FindAsync(dto.CustomerId);
        var products = await _db.Products.Where(p => dto.OrderItems.Select(i => i.ProductId).Contains(p.Id)).ToListAsync();

        (ok, result) = ValidateBusinessRules(customer, products, dto.OrderItems);
        if (!ok)
            return (ServiceResult<int>)result!;

        var order = await CreateOrder(dto, products);

        return ServiceResult<int>.Success(order.Id);
    }

    private async Task<Order> CreateOrder(OrderCreateDTO dto, List<Product> products)
    {
        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;
        foreach (var item in dto.OrderItems)
        {
            var product = products.First(p => p.Id == item.ProductId);

            orderItems.Add(new OrderItem {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
            totalAmount += product.Price * item.Quantity;
            product.Stock -= item.Quantity;
        }
        var order = new Order {
            CustomerId = dto.CustomerId,
            OrderDate = DateTime.UtcNow,
            OrderStatus = Order.Status.Pending,
            TotalAmount = totalAmount,
            OrderItems = orderItems
        };
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task<ServiceResult> UpdateOrderStatusAsync(int orderId, string newStatusStr)
    {
        bool ok = true;
        ServiceResult? result = null;

        (ok, result) = ValidateReadOrder(orderId);
        if (!ok)
            return (ServiceResult)result!;

        var order = await _db.Orders.FindAsync(orderId);

        if (!Enum.TryParse<Order.Status>(newStatusStr, true, out var newStatus))
            return ServiceResult.BadRequest("Invalid order status.");

        (ok, result) = ValidateStatusTransition(order!.OrderStatus, newStatus);
        if (!ok)
            return (ServiceResult)result!;

        await UpdateOrderStatus(order, newStatus);

        return ServiceResult.NoContent();
    }

    private async Task UpdateOrderStatus(Order order, Order.Status newStatus)
    {
        order.OrderStatus = newStatus;
        await _db.SaveChangesAsync();
    }

    private (bool ok, ServiceResult<int>? result) ValidateCreateRequest(OrderCreateDTO dto)
    {
        if (dto.CustomerId <= 0)
            return (false, ServiceResult<int>.BadRequest("Invalid customer ID {dto.CustomerId}."));
        if (dto.OrderItems.Count == 0)
            return (false, ServiceResult<int>.BadRequest("At least one order item is required."));
        foreach (var item in dto.OrderItems)
        {
            if (item.ProductId <= 0)
                return (false, ServiceResult<int>.BadRequest($"Invalid product ID {item.ProductId}."));
            if (item.Quantity <= 0)
                return (false, ServiceResult<int>.BadRequest($"Product quantity must be greater than zero."));
        }
        return (true, null);
    }

    private (bool ok, ServiceResult<int>? result) ValidateBusinessRules(Customer? customer, List<Product> products, List<OrderItemCreateDTO> items)
    {
        if (customer is null)
            return (false, ServiceResult<int>.NotFound("Customer not found."));

        var quantityByProductId = items.GroupBy(i => i.ProductId).ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));
        var productById = products.ToDictionary(p => p.Id);

        foreach (var (productId, quantityInOrder) in quantityByProductId)
        {
            if (!productById.TryGetValue(productId, out var product))
                return (false, ServiceResult<int>.NotFound($"Product with ID {productId} not found."));
            if (!product.IsActive)
                return (false, ServiceResult<int>.Conflict($"Product {product.Name} is not available."));
            if (product.Stock < quantityInOrder)
                return (false, ServiceResult<int>.Conflict($"Not enough stock for {product.Name}."));
        }

        return (true, null);
    }

    private (bool ok, ServiceResult? result) ValidateReadOrder(int orderId)
    {
        if (orderId <= 0)
            return (false, ServiceResult.BadRequest($"Invalid order ID {orderId}."));
        if (!_db.Orders.Any(o => o.Id == orderId))
            return (false, ServiceResult.NotFound($"Order {orderId} not found."));
        return (true, null);
    }

    private (bool ok, ServiceResult? result) ValidateStatusTransition(Order.Status currentStatus, Order.Status newStatus)
    {
        return newStatus switch
        {
            Order.Status.Shipped => currentStatus == Order.Status.Pending
                ? (true, null)
                : (false, ServiceResult.Conflict($"Can only transition to {Order.Status.Shipped} from {Order.Status.Pending}.")),
            Order.Status.Delivered => currentStatus == Order.Status.Shipped
                ? (true, null)
                : (false, ServiceResult.Conflict($"Can only transition to {Order.Status.Delivered} from {Order.Status.Shipped}.")),
            Order.Status.Cancelled => currentStatus == Order.Status.Pending
                ? (true, null)
                : (false, ServiceResult.Conflict($"Can only transition to {Order.Status.Cancelled} from {Order.Status.Pending}.")),
            Order.Status.Pending => (false, ServiceResult.Conflict($"Cannot transition back to {Order.Status.Pending} status.")),
            _ => (false, ServiceResult.BadRequest("Invalid order status."))
        };
    }
}
