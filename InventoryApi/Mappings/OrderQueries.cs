using InventoryApi.DTOs.Common;
using InventoryApi.DTOs.Orders;
using InventoryApi.Models;

namespace InventoryApi.Mappings;

public static class OrderQueries
{
    public static IQueryable<OrderSummaryDTO> SelectOrderSummary(this IQueryable<Order> q) =>
       q.Select(o => new OrderSummaryDTO(
           o.Id,
        new CustomerInfoDTO(
            o.Customer.Id,
            o.Customer.Name
        ),
        o.OrderDate,
        o.OrderStatus.ToString(),
        o.OrderItems.Count,
        o.TotalAmount
       ));

    public static IQueryable<OrderDetailDTO> SelectOrderDetail(this IQueryable<Order> q, int id) =>
    q.Where(o => o.Id == id)
        .Select(o => new OrderDetailDTO(
        o.Id,
        new CustomerInfoDTO(o.Customer.Id, o.Customer.Name),
        o.OrderDate,
        o.OrderStatus.ToString(),
        o.TotalAmount,
        o.OrderItems.Select(oi => new OrderItemDTO(
            oi.Id,
            new ProductInfoDTO(oi.Product.Id, oi.Product.Name),
            oi.Quantity,
            oi.UnitPrice
        )).ToList()
    ));
}
