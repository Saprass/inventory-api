namespace InventoryApi.DTOs.Customers;

public class CustomerCreateDTO
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
}
