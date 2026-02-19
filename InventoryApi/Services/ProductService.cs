using InventoryApi.Data;
using InventoryApi.DTOs.Products;
using InventoryApi.Models;

namespace InventoryApi.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db) => _db = db;

    public async Task<ServiceResult<int>> CreateProductAsync(ProductCreateDTO dto)
    {
        bool ok;
        ServiceResult<int>? result = null;

        (ok, result) = ValidateCreateProduct(dto);
        if (!ok)
            return (ServiceResult<int>)result!;

        Product product = new Product {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            IsActive = dto.IsActive
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return ServiceResult<int>.Created(product.Id, $"/products/{product.Id}");
    }

    public async Task<ServiceResult> UpdateProductAsync(int productId, ProductPatchDTO dto)
    {
        bool ok;
        ServiceResult? result = null;

        (ok, result) = ValidateUpdateProduct(productId, dto);
        if (!ok)
            return (ServiceResult)result!;

        var product = await _db.Products.FindAsync(productId);

        if (product is null) return ServiceResult.NotFound($"Product {productId} not found");

        if (dto.Name is not null) product.Name = dto.Name;
        if (dto.Description is not null) product.Description = dto.Description;
        if (dto.Price is not null) product.Price = dto.Price.Value;
        if (dto.Stock is not null) product.Stock = dto.Stock.Value;
        if (dto.IsActive is not null) product.IsActive = dto.IsActive.Value;

        await _db.SaveChangesAsync();

        return ServiceResult.NoContent();
    }

    public async Task<ServiceResult> DeactivateProductAsync(int productId, bool isActive)
    {
        bool ok;
        ServiceResult? result = null;

        (ok, result) = ValidateDeactivateProduct(productId);
        if (!ok)
            return (ServiceResult)result!;

        var product = await _db.Products.FindAsync(productId);

        if (product is null) return ServiceResult.NotFound($"Product {productId} not found");

        product.IsActive = false;

        await _db.SaveChangesAsync();

        return ServiceResult.NoContent();
    }

    private (bool ok, ServiceResult<int>? result) ValidateCreateProduct(ProductCreateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return (false, ServiceResult<int>.BadRequest("Product name is required."));
        if (dto.Price < 0)
            return (false, ServiceResult<int>.BadRequest("Price cannot be negative."));
        if (dto.Stock < 0)
            return (false, ServiceResult<int>.BadRequest("Stock cannot be negative."));
        return (true, null);
    }

    private (bool ok, ServiceResult? result) ValidateUpdateProduct(int productId, ProductPatchDTO dto)
    {
        if (productId <= 0)
            return (false, ServiceResult.BadRequest($"Invalid product ID {productId}."));
        if (dto.Name is not null && string.IsNullOrWhiteSpace(dto.Name))
            return (false, ServiceResult.BadRequest("Product name cannot be empty."));
        if (dto.Price is not null && dto.Price < 0)
            return (false, ServiceResult.BadRequest("Price cannot be negative."));
        if (dto.Stock is not null && dto.Stock < 0)
            return (false, ServiceResult.BadRequest("Stock cannot be negative."));
        return (true, null);
    }

    private (bool ok, ServiceResult? result) ValidateDeactivateProduct(int productId)
    {
        if (productId <= 0)
            return (false, ServiceResult.BadRequest($"Invalid product ID {productId}."));
        return (true, null);
    }
}
