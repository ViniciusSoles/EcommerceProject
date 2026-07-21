using Ecommerce.Application.DTOs.AuthDtos;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Application.DTOs.Auth;
using ECommerceApi.Application.Services;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using ECommerceApi.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace ECommerceApi.Tests.Application;

public class AuthServiceTests
{
    private readonly IUserRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _configuration = Substitute.For<IConfiguration>();

        _configuration["Jwt:SecretKey"].Returns("super-secret-key-minimum-32-chars!!");
        _configuration["Jwt:Issuer"].Returns("ECommerceApi");
        _configuration["Jwt:Audience"].Returns("ECommerceApiUsers");
        _configuration["Jwt:AccessTokenExpirationMinutes"].Returns("15");
        _configuration["Jwt:RefreshTokenExpirationDays"].Returns("7");

        _service = new AuthService(_repository, _configuration);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFail_WhenEmailAlreadyExists()
    {
        _repository.EmailExistsAsync(Arg.Any<string>()).Returns(true);

        var result = await _service.RegisterAsync(new RegisterDto
        {
            Name = "Person",
            Email = "Person@mail.com",
            Password = "12345678"
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("already registered");
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnOk_WhenValid()
    {
        _repository.EmailExistsAsync(Arg.Any<string>()).Returns(false);

        var result = await _service.RegisterAsync(new RegisterDto
        {
            Name = "Person",
            Email = "Person@gmail.com",
            Password = "12345678"
        });

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).AddAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFail_WhenUserNotFound()
    {
        _repository.GetByEmailAsync(Arg.Any<string>()).ReturnsNull();

        var result = await _service.LoginAsync(new LoginDto
        {
            Email = "notfound@gmail.com",
            Password = "12345678"
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFail_WhenPasswordIsWrong()
    {
        var user = new User("Person", new Email("Peson@gmail.com"),
            BCrypt.Net.BCrypt.HashPassword("correctpassword"), "Customer");

        _repository.GetByEmailAsync(Arg.Any<string>()).Returns(user);

        var result = await _service.LoginAsync(new LoginDto
        {
            Email = "Person@gmail.com",
            Password = "wrongpassword"
        });

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnTokens_WhenValid()
    {
        var user = new User("Person", new Email("Person@gmail.com"),
            BCrypt.Net.BCrypt.HashPassword("12345678"), "Customer");

        _repository.GetByEmailAsync(Arg.Any<string>()).Returns(user);

        var result = await _service.LoginAsync(new LoginDto
        {
            Email = "Person@gmail.com",
            Password = "12345678"
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        result.Value.AccessTokenExpiresAt.Should().BeAfter(DateTime.UtcNow);
        result.Value.RefreshTokenExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task RevokeAsync_ShouldReturnFail_WhenTokenNotFound()
    {
        _repository.GetByRefreshTokenAsync(Arg.Any<string>()).ReturnsNull();

        var result = await _service.RevokeAsync("invalid-token");

        result.IsFailed.Should().BeTrue();
    }
}