using InventoryApi.Data;
using InventoryApi.DTOs.Products;
using InventoryApi.Models;
using InventoryApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly AppDbContext _db;

    public ProductsController(IProductService service, AppDbContext db)
    {
        _service = service;
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.Products
            .AsNoTracking()
            .Select(x => new ProductDTO(x))
            .ToListAsync());
    }

    [HttpGet("{id:int:min(1)}")]
    public async Task<IActionResult> GetById(int id)
    {
        return await _db.Products.FindAsync(id)
        is Product product
            ? Ok(new ProductDTO(product))
            : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDTO createProduct)
    {
        ServiceResult<ProductDTO> result = await _service.CreateProductAsync(createProduct);

        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int:min(1)}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductPatchDTO patchedProduct)
    {
        ServiceResult result = await _service.UpdateProductAsync(id, patchedProduct);
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int:min(1)}/deactivate")]
    public async Task<IActionResult> Deactivate(int id)
    {
        ServiceResult result = await _service.DeactivateProductAsync(id, false);
        return result.ToActionResult(this);
    }
}
