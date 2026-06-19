using Azure;
using Azure.Core;
using Ecommerce.Application.DTOs.AuthDtos;
using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _service.RegisterAsync(dto);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Registration failed.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        return Created("/api/auth/login", new { message = "Registration successful. Please log in." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _service.LoginAsync(dto);

        if (result.IsFailed)
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid credentials.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status401Unauthorized
            });

        SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresAt);

        return Ok(new
        {
            result.Value.AccessToken,
            result.Value.AccessTokenExpiresAt
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new ProblemDetails
            {
                Title = "Refresh token not found.",
                Status = StatusCodes.Status401Unauthorized
            });

        var result = await _service.RefreshAsync(refreshToken);

        if (result.IsFailed)
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid token.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status401Unauthorized
            });

        SetRefreshTokenCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiresAt);

        return Ok(new
        {
            result.Value.AccessToken,
            result.Value.AccessTokenExpiresAt
        });
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<ActionResult> Revoke()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new ProblemDetails
            {
                Title = "Refresh token not found.",
                Status = StatusCodes.Status400BadRequest
            });

        var result = await _service.RevokeAsync(refreshToken);

        if (result.IsFailed)
            return BadRequest(new ProblemDetails
            {
                Title = "Failed to revoke token.",
                Detail = result.Errors.First().Message,
                Status = StatusCodes.Status400BadRequest
            });

        Response.Cookies.Delete("refreshToken");

        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken, DateTime expiresAt)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt
        });
    }
}