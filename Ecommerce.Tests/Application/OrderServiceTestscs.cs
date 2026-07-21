using Ecommerce.Application.DTOs.OrderDtos;
using Ecommerce.Domain.Enums;
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

public class OrderServiceTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly OrderService _service;

    private readonly Guid _userId = Guid.NewGuid();

    public OrderServiceTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _cartRepository = Substitute.For<ICartRepository>();
        _productRepository = Substitute.For<IProductRepository>();

        _service = new OrderService(
            _orderRepository,
            _cartRepository,
            _productRepository);
    }

    private CreateOrderDto CreateOrderDto() => new CreateOrderDto
    {
        Street = "Rua A",
        Number = "123",
        City = "São Paulo",
        State = "SP",
        ZipCode = "01000-000"
    };

    [Fact]
    public async Task CreateAsync_ShouldReturnFail_WhenCartIsEmpty()
    {
        _cartRepository.GetByUserIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.CreateAsync(_userId, CreateOrderDto());

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Cart is empty");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFail_WhenNotFound()
    {
        _orderRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.GetByIdAsync(Guid.NewGuid(), _userId, false);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("not found");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFail_WhenCustomerTriesToAccessAnotherUsersOrder()
    {
        var otherUserId = Guid.NewGuid();
        var order = new Order(
            otherUserId,
            new Address("Rua A", "123", "SP", "SP", "01000-000"),
            new List<OrderItem>
            {
                new OrderItem(Guid.Empty, Guid.NewGuid(), "Notebook", 1, new Money(1000m))
            });

        _orderRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(order);

        var result = await _service.GetByIdAsync(order.Id, _userId, isAdmin: false);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("permission");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenAdminAccessesAnyOrder()
    {
        var otherUserId = Guid.NewGuid();
        var order = new Order(
            otherUserId,
            new Address("Rua A", "123", "SP", "SP", "01000-000"),
            new List<OrderItem>
            {
                new OrderItem(Guid.Empty, Guid.NewGuid(), "Notebook", 1, new Money(1000m))
            });

        _orderRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(order);

        var result = await _service.GetByIdAsync(order.Id, _userId, isAdmin: true);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnFail_WhenOrderNotFound()
    {
        _orderRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.UpdateStatusAsync(Guid.NewGuid(), OrderStatus.Paid);

        result.IsFailed.Should().BeTrue();
    }
}