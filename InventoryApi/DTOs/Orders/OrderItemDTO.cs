using InventoryApi.DTOs.Common;

namespace InventoryApi.DTOs.Orders;

public class OrderItemDTO
{
    public int Id { get; set; }
    public ProductInfoDTO Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;

    public OrderItemDTO(int id, ProductInfoDTO product, int quantity, decimal unitPrice) =>
        (Id, Product, Quantity, UnitPrice) = (id, product, quantity, unitPrice);
}
