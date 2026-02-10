using InventoryApi.Models;
using InventoryApi.DTOs.Common;

namespace InventoryApi.DTOs.Orders;

public class OrderSummaryDTO
{
    public struct CustomerInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public CustomerInfo(Customer customer) =>
            (Id, Name) = (customer.Id, customer.Name);
    }

    public int Id { get; set; }
    public CustomerInfoDTO Customer { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public string OrderStatus { get; set; } = null!;
    private ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    //public int OrderCount => OrderItems.Count;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }


    public OrderSummaryDTO(int id, CustomerInfoDTO customer, DateTime orderDate, string status, int orderCount, decimal totalAmount) =>
        (Id, Customer, OrderDate, OrderStatus, OrderCount, TotalAmount) =
        (id, customer, orderDate, status, orderCount, totalAmount);
}
