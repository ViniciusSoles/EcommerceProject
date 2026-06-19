using Ecommerce.Application.DTOs.ReviewDtos;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceApi.API.Controllers;

[ApiController]
[Route("api")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _service;

    public ReviewsController(IReviewService service)
    {
        _service = service;
    }

    [HttpGet("products/{productId}/reviews")]
    public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetByProduct(Guid productId)
    {
        var result = await _service.GetByProductIdAsync(productId);
        return Ok(result.Value);
    }

    [HttpPost("products/{productId}/reviews")]
    [Authorize]
    public async Task<ActionResult<ReviewResponseDto>> Create(
        Guid productId, [FromBody] CreateReviewDto dto)
    {
        var result = await _service.CreateAsync(GetUserId(), productId, dto);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to create review.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return CreatedAtAction(nameof(GetByProduct), new { productId }, result.Value);
    }

    [HttpDelete("reviews/{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id, GetUserId(), IsAdmin());

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to delete review.",
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