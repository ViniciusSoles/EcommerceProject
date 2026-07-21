using Ecommerce.Application.DTOs.PaymentDtos;
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

public class PaymentServiceTests
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _paymentRepository = Substitute.For<IPaymentRepository>();
        _orderRepository = Substitute.For<IOrderRepository>();
        _service = new PaymentService(_paymentRepository, _orderRepository);
    }

    private Order CreatePendingOrder()
    {
        return new Order(
            Guid.NewGuid(),
            new Address("Rua A", "123", "SP", "SP", "01000-000"),
            new List<OrderItem>
            {
                new OrderItem(Guid.Empty, Guid.NewGuid(), "Notebook", 1, new Money(1000m))
            });
    }

    [Fact]
    public async Task ProcessAsync_ShouldReturnFail_WhenOrderNotFound()
    {
        _orderRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.ProcessAsync(Guid.NewGuid(), new CreatePaymentDto
        {
            Method = PaymentMethod.CreditCard
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Order not found");
    }

    [Fact]
    public async Task ProcessAsync_ShouldReturnFail_WhenOrderIsNotPending()
    {
        var order = CreatePendingOrder();
        order.UpdateStatus(OrderStatus.Paid);
        _orderRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(order);

        var result = await _service.ProcessAsync(order.Id, new CreatePaymentDto
        {
            Method = PaymentMethod.CreditCard
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("not pending");
    }

    [Fact]
    public async Task ProcessAsync_ShouldReturnPayment_WhenOrderIsPending()
    {
        var order = CreatePendingOrder();
        _orderRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(order);

        var result = await _service.ProcessAsync(order.Id, new CreatePaymentDto
        {
            Method = PaymentMethod.Pix
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().BeOneOf(
            PaymentStatus.Approved.ToString(),
            PaymentStatus.Refused.ToString());
        await _paymentRepository.Received(1).AddAsync(Arg.Any<Payment>());
    }
}