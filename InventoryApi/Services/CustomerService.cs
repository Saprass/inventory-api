using InventoryApi.Data;
using InventoryApi.DTOs.Customers;
using InventoryApi.Models;

namespace InventoryApi.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;

    public CustomerService(AppDbContext db) => _db = db;

    public async Task<ServiceResult<int>> CreateCustomerAsync(CustomerCreateDTO dto)
    {
        bool ok;
        ServiceResult<int>? result = null;

        (ok, result) = ValidateCreateRequest(dto);
        if (!ok)
            return (ServiceResult<int>)result!;

        Customer customer = new Customer {
            Name = dto.Name,
            Email = dto.Email,
            Address = dto.Address,
            Phone = dto.Phone
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return ServiceResult<int>.Created(customer.Id, $"/customers/{customer.Id}");
    }

    public async Task<ServiceResult> UpdateCustomerAsync(int customerId, CustomerPatchDTO dto)
    {
        bool ok;
        ServiceResult? result = null;

        (ok, result) = ValidateUpdateRequest(customerId, dto);
        if (!ok)
            return (ServiceResult)result!;

        var customer = await _db.Customers.FindAsync(customerId);

        if (customer is null) return ServiceResult.NotFound($"Customer {customerId} not found");

        if (dto.Name is not null) customer.Name = dto.Name;
        if (dto.Email is not null) customer.Email = dto.Email;
        if (dto.Address is not null) customer.Address = dto.Address;
        if (dto.Phone is not null) customer.Phone = dto.Phone;

        await _db.SaveChangesAsync();

        return ServiceResult.NoContent();
    }

    private (bool ok, ServiceResult<int>? result) ValidateCreateRequest(CustomerCreateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return (false, ServiceResult<int>.BadRequest("Customer name is required."));
        if (string.IsNullOrWhiteSpace(dto.Email))
            return (false, ServiceResult<int>.BadRequest("Customer email is required."));
        return (true, null);
    }

    private (bool ok, ServiceResult? result) ValidateUpdateRequest(int customerId, CustomerPatchDTO dto)
    {
        if (customerId <= 0)
            return (false, ServiceResult.BadRequest($"Invalid customer ID {customerId}."));
        if (dto.Name is not null && string.IsNullOrWhiteSpace(dto.Name))
            return (false, ServiceResult.BadRequest("Customer name cannot be empty."));
        if (dto.Email is not null && string.IsNullOrWhiteSpace(dto.Email))
            return (false, ServiceResult.BadRequest("Customer email cannot be empty."));
        return (true, null);
    }
}
