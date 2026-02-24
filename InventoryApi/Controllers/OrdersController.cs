using InventoryApi.Data;
using InventoryApi.DTOs.Orders;
using InventoryApi.Mappings;
using InventoryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    private readonly AppDbContext _db;

    public OrdersController(IOrderService service, AppDbContext db)
    {
        _service = service;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _db.Orders
            .SelectOrderSummary()
            .ToListAsync();
        
        return Ok(orders);
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetById(int id)
    {
        var orderDto = await _db.Orders
            .SelectOrderDetail(id)
            .FirstOrDefaultAsync();
        
        return orderDto is null ? NotFound() : Ok(orderDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderCreateDTO createOrder)
    {
        ServiceResult<OrderDetailDTO> result = await _service.CreateOrderAsync(createOrder);
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int:min(1)}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatusUpdateDTO oStatusDTO)
    {
        ServiceResult result = await _service.UpdateOrderStatusAsync(id, oStatusDTO.Status);
        return result.ToActionResult(this);
    }
}
