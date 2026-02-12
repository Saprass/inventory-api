namespace InventoryApi.DTOs.Orders;

public class OrderCreateDTO
{
    public int CustomerId { get; set; }
    public List<OrderItemCreateDTO> OrderItems { get; set; } = new List<OrderItemCreateDTO>();
}
