using InventoryApi.DTOs.Orders;
using InventoryApi.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Services;

public interface IOrderService
{
    public Task<ServiceResult<int>> CreateOrderAsync(OrderCreateDTO dto);
    public Task<ServiceResult> UpdateOrderStatusAsync(int orderId, string newStatus);
}
