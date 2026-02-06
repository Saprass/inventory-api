namespace InventoryApi.DTOs.Customers;

public class CustomerPatchDTO
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
}
