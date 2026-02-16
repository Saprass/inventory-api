using InventoryApi.DTOs.Orders;
using InventoryApi.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Services;

public interface IOrderService
{
    public Task<(bool ok, string? error, int? orderId)> CreateOrderAsync(OrderCreateDTO dto, AppDbContext db);
    public Task<(bool ok, string? error)> UpdateOrderStatusAsync(int orderId, string newStatus, AppDbContext db);
}
