namespace InventoryApi.DTOs.Orders;

public class OrderItemCreateDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
