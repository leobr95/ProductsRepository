using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;     
using Swashbuckle.AspNetCore.Filters;
using RealEstate.Application;
using RealEstate.Application.DTOs;
using RealEstate.Application.Interfaces;
using RealEstate.Api.Swagger.Examples;

namespace RealEstate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _svc;
    private readonly IStringLocalizer<SharedResources> _loc;

    public ProductsController(IProductService svc, IStringLocalizer<SharedResources> loc)
    {
        _svc = svc;
        _loc = loc;
    }

    /// <summary>Get all products (optional search by name)</summary>
    /// <param name="q">Search by product name (contains)</param>
    [HttpGet]
    [SwaggerOperation(Summary = "Get product list", Description = "Returns all products. Optional search by name.")]
    [SwaggerResponse(200, "OK", typeof(IEnumerable<ProductDto>))]
    [SwaggerResponseExample(200, typeof(ProductListExample))]         
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] string? q, CancellationToken ct)
        => Ok(await _svc.GetAllAsync(q, ct));

    /// <summary>Get product by id</summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get product by id")]
    [SwaggerResponse(200, "OK", typeof(ProductDto))]
    [SwaggerResponseExample(200, typeof(ProductDtoExample))]        
    [SwaggerResponse(404, "Not found")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken ct)
    {
        var dto = await _svc.GetByIdAsync(id, ct);
        if (dto is null) return NotFound(new { message = _loc["Error.NotFound"] });
        return Ok(dto);
    }

    /// <summary>Create product</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create product")]
    [SwaggerRequestExample(typeof(ProductCreateDto), typeof(ProductCreateDtoExample))]  
    [SwaggerResponse(201, "Created", typeof(object))]
    [SwaggerResponseExample(201, typeof(ProductDtoExample))]
    [SwaggerResponse(400, "Validation errors")]
    public async Task<ActionResult> Create([FromBody] ProductCreateDto dto, CancellationToken ct)
    {
        var (ok, id, errors) = await _svc.CreateAsync(dto, ct);
        if (!ok) return BadRequest(new { errors });
        return CreatedAtAction(nameof(GetById), new { id = id!.Value }, new { id });
    }

    /// <summary>Update product</summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Update product")]
    [SwaggerRequestExample(typeof(ProductUpdateDto), typeof(ProductUpdateDtoExample))] 
    [SwaggerResponse(204, "No content")]
    [SwaggerResponse(400, "Validation errors")]
    [SwaggerResponse(404, "Not found")]
    public async Task<ActionResult> Update(int id, [FromBody] ProductUpdateDto dto, CancellationToken ct)
    {
        var (ok, errors) = await _svc.UpdateAsync(id, dto, ct);
        if (!ok) return BadRequest(new { errors });
        return NoContent();
    }

    /// <summary>Delete product</summary>
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete product")]
    [SwaggerResponse(204, "No content")]
    [SwaggerResponse(404, "Not found")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _svc.DeleteAsync(id, ct);
        if (!ok) return NotFound(new { message = _loc["Error.NotFound"] });
        return NoContent();
    }
}
