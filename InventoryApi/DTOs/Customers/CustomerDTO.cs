using InventoryApi.Models;

namespace InventoryApi.DTOs.Customers;

public class CustomerDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }

    public CustomerDTO(Customer customer) =>
        (Id, Name, Email, Address, Phone, CreatedAt) =
        (customer.Id, customer.Name, customer.Email, customer.Address, customer.Phone, customer.CreatedAt);
}
