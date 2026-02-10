namespace InventoryApi.DTOs.Common;

public class CustomerInfoDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public CustomerInfoDTO(int id, string name) => (Id, Name) = (id, name);
}
