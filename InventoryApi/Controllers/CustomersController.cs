using InventoryApi.Data;
using InventoryApi.DTOs.Customers;
using InventoryApi.Models;
using InventoryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Controllers;

[ApiController]
[Route("customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    private readonly AppDbContext _db;

    public CustomersController(ICustomerService service, AppDbContext db)
    {
        _service = service;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.Customers
            .AsNoTracking()
            .Select(x => new CustomerDTO(x))
            .ToListAsync());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetById(int id)
    {
        return await _db.Customers.FindAsync(id)
        is Customer customer
            ? Ok(new CustomerDTO(customer))
            : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDTO createCustomer)
    {
        ServiceResult<CustomerDTO> result = await _service.CreateCustomerAsync(createCustomer);
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerPatchDTO patchedCustomer)
    {
        ServiceResult result = await _service.UpdateCustomerAsync(id, patchedCustomer);
        return result.ToActionResult(this);
    }
}
