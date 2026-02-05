using InventoryApi.Models;

namespace InventoryApi.DTOs.Products;

public class ProductDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }

    public ProductDTO(Product product) =>
        (Id, Name, Description, Price, Stock, IsActive) = 
        (product.Id, product.Name, product.Description, product.Price, product.Stock, product.IsActive);
}