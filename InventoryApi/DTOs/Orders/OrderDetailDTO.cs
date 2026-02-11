using InventoryApi.DTOs.Common;

namespace InventoryApi.DTOs.Orders;

public class OrderDetailDTO
{
    public int Id { get; set; }
    public CustomerInfoDTO Customer { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public string OrderStatus { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();

    public OrderDetailDTO(int id, CustomerInfoDTO customer, DateTime orderDate, string orderStatus, decimal totalAmount, List<OrderItemDTO> orderItems)
    => (Id, Customer, OrderDate, OrderStatus, TotalAmount, OrderItems) = (id, customer, orderDate, orderStatus, totalAmount, orderItems);
}
