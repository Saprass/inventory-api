using InventoryApi.DTOs.Customers;

namespace InventoryApi.Services;

public interface ICustomerService
{
    public Task<ServiceResult<CustomerDTO>> CreateCustomerAsync(CustomerCreateDTO dto);
    public Task<ServiceResult> UpdateCustomerAsync(int customerId, CustomerPatchDTO dto);
}
