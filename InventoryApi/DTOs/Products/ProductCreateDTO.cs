namespace InventoryApi.DTOs.Products;

public class ProductCreateDTO
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
}