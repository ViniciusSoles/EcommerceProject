using Ecommerce.Application.DTOs;
using Ecommerce.Application.DTOs.ProductDtos;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace ECommerceApi.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductResponseDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? search = null)
    {
        var pagination = new PaginationParams
        {
            Page = page,
            PageSize = pageSize
        };

        var result = await _service.GetAllAsync(pagination, categoryId, search);
        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result.IsFailed)
            return NotFound(new ProblemDetails
            {
                Title = "Product not found.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status404NotFound
            });

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to create product.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to update product.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to delete product.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Deactivate(Guid id)
    {
        var result = await _service.DeactivateAsync(id);

        if (result.IsFailed)
            return NotFound(new ProblemDetails
            {
                Title = "Product not found.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status404NotFound
            });

        return NoContent();
    }

    [HttpPatch("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Activate(Guid id)
    {
        var result = await _service.ActivateAsync(id);

        if (result.IsFailed)
            return NotFound(new ProblemDetails
            {
                Title = "Product not found.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status404NotFound
            });

        return NoContent();
    }
}