using Ecommerce.Application.DTOs;
using Ecommerce.Application.DTOs.OrderDtos;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApi.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> Create([FromBody] CreateOrderDto dto)
    {
        var result = await _service.CreateAsync(GetUserId(), dto);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to create order.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id, GetUserId(), IsAdmin());

        if (result.IsFailed)
            return NotFound(new ProblemDetails
            {
                Title = "Order not found.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status404NotFound
            });

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderResponseDto>>> GetMyOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationParams { Page = page, PageSize = pageSize };
        var result = await _service.GetByUserIdAsync(GetUserId(), pagination);

        return Ok(result.Value);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PagedResult<OrderResponseDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationParams { Page = page, PageSize = pageSize };
        var result = await _service.GetAllAsync(pagination);

        return Ok(result.Value);
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        var result = await _service.UpdateStatusAsync(id, dto.Status);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to update order status.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return NoContent();
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private bool IsAdmin() =>
        User.IsInRole("Admin");
}