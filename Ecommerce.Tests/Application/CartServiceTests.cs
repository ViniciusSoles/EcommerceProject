using Ecommerce.Application.DTOs.CartDtos;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Application.Services;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using ECommerceApi.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace ECommerceApi.Tests.Application;

public class CartServiceTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly CartService _service;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    public CartServiceTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _service = new CartService(_cartRepository, _productRepository);
    }

    private Product CreateProduct(int stock = 10) =>
        new Product("Notebook", "Descrição", new Money(1000m), stock, _categoryId);

    [Fact]
    public async Task AddItemAsync_ShouldReturnFail_WhenProductNotFound()
    {
        _productRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.AddItemAsync(_userId, new AddCartItemDto
        {
            ProductId = _productId,
            Quantity = 1
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Product not found");
    }

    [Fact]
    public async Task AddItemAsync_ShouldReturnFail_WhenProductIsInactive()
    {
        var product = CreateProduct();
        product.Deactivate();
        _productRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(product);

        var result = await _service.AddItemAsync(_userId, new AddCartItemDto
        {
            ProductId = _productId,
            Quantity = 1
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("not available");
    }

    [Fact]
    public async Task AddItemAsync_ShouldReturnFail_WhenInsufficientStock()
    {
        var product = CreateProduct(stock: 5);
        _productRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(product);
        _cartRepository.GetByUserIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.AddItemAsync(_userId, new AddCartItemDto
        {
            ProductId = _productId,
            Quantity = 10
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Insufficient stock");
    }

    [Fact]
    public async Task AddItemAsync_ShouldCreateCart_WhenUserHasNoCart()
    {
        var product = CreateProduct();
        _productRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(product);
        _cartRepository.GetByUserIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.AddItemAsync(_userId, new AddCartItemDto
        {
            ProductId = _productId,
            Quantity = 1
        });

        await _cartRepository.Received(1).AddAsync(Arg.Any<Cart>());
    }

    [Fact]
    public async Task ClearAsync_ShouldReturnFail_WhenCartNotFound()
    {
        _cartRepository.GetByUserIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.ClearAsync(_userId);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Cart not found");
    }
}