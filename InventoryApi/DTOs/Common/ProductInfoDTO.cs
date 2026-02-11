namespace InventoryApi.DTOs.Common;

public class ProductInfoDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ProductInfoDTO(int id, string name) => (Id, Name) = (id, name);
}
