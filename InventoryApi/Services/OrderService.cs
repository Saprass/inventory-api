using InventoryApi.Data;
using InventoryApi.DTOs.Orders;
using InventoryApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace InventoryApi.Services;

public class OrderService : IOrderService
{
    public async Task<(bool ok, string? error, int? orderId)> CreateOrderAsync(OrderCreateDTO dto, AppDbContext db)
    {
        bool ok = true;
        string? error = null;

        (ok, error) = ValidateRequest(dto);
        if (!ok)
            return (false, error, null);

        var customer = await db.Customers.FindAsync(dto.CustomerId);
        var products = await db.Products.Where(p => dto.OrderItems.Select(i => i.ProductId).Contains(p.Id)).ToListAsync();

        (ok, error) = ValidateBusinessRules(customer, products, dto.OrderItems);
        if (!ok)
            return (false, error, null);

        var order = await CreateOrder(dto, products, db);

        return (true, null, order.Id);
    }

    private static async Task<Order> CreateOrder(OrderCreateDTO dto, List<Product> products, AppDbContext db)
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
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return order;
    }

    public async Task<ServiceResult> UpdateOrderStatusAsync(int orderId, string newStatusStr, AppDbContext db)
    {
        bool ok = true;
        ServiceResult? result = null;

        (ok, result) = ValidateReadOrder(orderId, db);
        if (!ok)
            return (ServiceResult)result!;

        var order = await db.Orders.FindAsync(orderId);

        if (!Enum.TryParse<Order.Status>(newStatusStr, true, out var newStatus))
            return ServiceResult.BadRequest("Invalid order status.");

        (ok, result) = ValidateStatusTransition(order!.OrderStatus, newStatus);
        if (!ok)
            return (ServiceResult)result!;

        await UpdateOrderStatus(order, newStatus, db);

        return ServiceResult.NoContent();
    }

    private static async Task UpdateOrderStatus(Order order, Order.Status newStatus, AppDbContext db)
    {
        order.OrderStatus = newStatus;
        await db.SaveChangesAsync();
    }

    private static (bool ok, string? error) ValidateRequest(OrderCreateDTO dto)
    {
        if (dto.CustomerId <= 0)
            return (false, $"Invalid customer ID {dto.CustomerId}.");
        if (dto.OrderItems.Count == 0)
            return (false, "At least one order item is required.");
        foreach (var item in dto.OrderItems)
        {
            if (item.ProductId <= 0)
                return (false, $"Invalid product ID {item.ProductId}.");
            if (item.Quantity <= 0)
                return (false, $"Product quantity must be greater than zero.");
        }
        return (true, null);
    }

    private static (bool ok, string? error) ValidateBusinessRules(Customer? customer, List<Product> products, List<OrderItemCreateDTO> items)
    {
        if (customer is null)
            return (false, "Customer not found.");

        var quantityByProductId = items.GroupBy(i => i.ProductId).ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));
        var productById = products.ToDictionary(p => p.Id);

        foreach (var (productId, quantityInOrder) in quantityByProductId)
        {
            if (!productById.TryGetValue(productId, out var product))
                return (false, $"Product with ID {productId} not found.");
            if (!product.IsActive)
                return (false, $"Product {product.Name} is not available.");
            if (product.Stock < quantityInOrder)
                return (false, $"Not enough stock for {product.Name}.");
        }

        return (true, null);
    }

    private static (bool ok, ServiceResult? result) ValidateReadOrder(int orderId, AppDbContext db)
    {
        if (orderId <= 0)
            return (false, ServiceResult.BadRequest($"Invalid order ID {orderId}."));
        if (!db.Orders.Any(o => o.Id == orderId))
            return (false, ServiceResult.NotFound($"Order {orderId} not found."));
        return (true, null);
    }

    private static (bool ok, ServiceResult? result) ValidateStatusTransition(Order.Status currentStatus, Order.Status newStatus)
    {
        //if (currentStatus == newStatus)
        //    return (false, $"Order is already in {currentStatus} status.");
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
