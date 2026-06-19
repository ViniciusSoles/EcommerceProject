using Ecommerce.Application.DTOs.PaymentDtos;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service)
    {
        _service = service;
    }

    [HttpPost("{orderId}")]
    public async Task<ActionResult<PaymentResponseDto>> Process(
        Guid orderId, [FromBody] CreatePaymentDto dto)
    {
        var result = await _service.ProcessAsync(orderId, dto);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Payment processing failed.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return Ok(result.Value);
    }
}