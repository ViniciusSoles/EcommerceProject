using Ecommerce.Application.DTOs.CartDtos;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApi.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<CartResponseDto>> Get()
    {
        var result = await _service.GetByUserIdAsync(GetUserId());
        return Ok(result.Value);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponseDto>> AddItem([FromBody] AddCartItemDto dto)
    {
        var result = await _service.AddItemAsync(GetUserId(), dto);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to add item to cart.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return Ok(result.Value);
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<ActionResult<CartResponseDto>> UpdateItem(
        Guid cartItemId, [FromBody] UpdateCartItemDto dto)
    {
        var result = await _service.UpdateItemAsync(GetUserId(), cartItemId, dto);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to update item.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return Ok(result.Value);
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<ActionResult> RemoveItem(Guid cartItemId)
    {
        var result = await _service.RemoveItemAsync(GetUserId(), cartItemId);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to remove item.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> Clear()
    {
        var result = await _service.ClearAsync(GetUserId());

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to clear cart.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return NoContent();
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}