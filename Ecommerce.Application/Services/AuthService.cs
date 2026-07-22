using Ecommerce.Application.DTOs.AuthDtos;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _repository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    public async Task<Result> RegisterAsync(RegisterDto dto)
    {
        if (await _repository.EmailExistsAsync(dto.Email))
            return Result.Fail(
                new Error("Email já cadastrado.").WithMetadata("ErrorCode", "EMAIL_ALREADY_EXISTS"));

        Email email;
        try
        {
            email = new Email(dto.Email);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(
                new Error("Credenciais Inválidas").WithMetadata("ErrorCode", "INVALID_CREDENTIALS"));
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new User(dto.Name, email, passwordHash, "Customer");

        await _repository.AddAsync(user);

        return Result.Ok();
    }

    public async Task<Result<TokenResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _repository.GetByEmailAsync(dto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Result.Fail(
              new Error("Credenciais inválidas").WithMetadata("ErrorCode", "INVALID_CREDENTIALS"));

        var tokens = await GenerateTokens(user);
        return Result.Ok(tokens);
    }

    public async Task<Result<TokenResponseDto>> RefreshAsync(string refreshToken)
    {
        var refreshHash = HashToken(refreshToken);
        var user = await _repository.GetByRefreshTokenAsync(refreshHash);

        if (user is null || !user.IsRefreshTokenValid(refreshHash))
            return Result.Fail(
              new Error("Acesso negado ou expirado.").WithMetadata("ErrorCode", "DENIED_OR_EXPIRED_ACESS"));

        var tokens = await GenerateTokens(user);
        return Result.Ok(tokens);
    }

    public async Task<Result> RevokeAsync(string refreshToken)
    {
        var refreshHash = HashToken(refreshToken);
        var user = await _repository.GetByRefreshTokenAsync(refreshHash);

        if (user is null)
            return Result.Fail(
              new Error("Acesso negado ou expirado.").WithMetadata("ErrorCode", "DENIED_OR_EXPIRED_ACESS"));

        user.RevokeRefreshToken();
        await _repository.UpdateAsync(user);

        return Result.Ok();
    }

    private async Task<TokenResponseDto> GenerateTokens(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshHash = HashToken(refreshToken);
        var refreshExpiry = DateTime.UtcNow.AddDays(
            int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"]!));

        user.SetRefreshToken(refreshHash, refreshExpiry);
        await _repository.UpdateAsync(user);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]!)),
            RefreshTokenExpiresAt = refreshExpiry
        };
    }

    private string GenerateAccessToken(User user)
    {
        var secretKey = _configuration["Jwt:SecretKey"]!;
        var issuer = _configuration["Jwt:Issuer"]!;
        var audience = _configuration["Jwt:Audience"]!;
        var expiration = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email.Value),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiration),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}

