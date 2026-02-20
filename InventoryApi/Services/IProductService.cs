using InventoryApi.DTOs.Products;

namespace InventoryApi.Services;

public interface IProductService
{
    public Task<ServiceResult<ProductDTO>> CreateProductAsync(ProductCreateDTO dto);
    public Task<ServiceResult> UpdateProductAsync(int productId, ProductPatchDTO dto);
    public Task<ServiceResult> DeactivateProductAsync(int productId, bool isActive);
}
