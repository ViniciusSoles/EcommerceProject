using Ecommerce.Domain.Enums;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Exceptions;
using ECommerceApi.Domain.ValueObjects;
using EcommerceProject;
using FluentAssertions;
using Xunit;

namespace ECommerceApi.Tests.Domain;

public class OrderTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();

    private Address CreateAddress() =>
        new Address("Rua A", "123", "São Paulo", "SP", "01000-000");

    private List<OrderItem> CreateItems(int quantity = 1) =>
        new List<OrderItem>
        {
            new OrderItem(Guid.Empty, _productId, "Notebook", quantity, new Money(1000m))
        };

    [Fact]
    public void Constructor_ShouldCreateOrder_WhenValid()
    {
        var order = new Order(_userId, CreateAddress(), CreateItems());

        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().HaveCount(1);
        order.Total.Amount.Should().Be(1000m);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenNoItems()
    {
        var act = () => new Order(_userId, CreateAddress(), new List<OrderItem>());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Order must have at least one item*");
    }

    [Fact]
    public void UpdateStatus_ShouldTransition_FromPendingToPaid()
    {
        var order = new Order(_userId, CreateAddress(), CreateItems());

        order.UpdateStatus(OrderStatus.Paid);

        order.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public void UpdateStatus_ShouldThrow_WhenInvalidTransition()
    {
        var order = new Order(_userId, CreateAddress(), CreateItems());

        var act = () => order.UpdateStatus(OrderStatus.Delivered);

        act.Should().Throw<InvalidOrderStatusTransitionException>();
    }

    [Fact]
    public void Cancel_ShouldCancelOrder_WhenPending()
    {
        var order = new Order(_userId, CreateAddress(), CreateItems());

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenOrderIsShipped()
    {
        var order = new Order(_userId, CreateAddress(), CreateItems());
        order.UpdateStatus(OrderStatus.Paid);
        order.UpdateStatus(OrderStatus.Processing);
        order.UpdateStatus(OrderStatus.Shipped);

        var act = () => order.Cancel();

        act.Should().Throw<CannotCancelShippedOrderException>();
    }

    [Fact]
    public void Total_ShouldBeCalculatedCorrectly_WithMultipleItems()
    {
        var items = new List<OrderItem>
        {
            new OrderItem(Guid.Empty, _productId, "Notebook", 2, new Money(1000m)),
            new OrderItem(Guid.Empty, Guid.NewGuid(), "Mouse", 3, new Money(100m))
        };

        var order = new Order(_userId, CreateAddress(), items);

        order.Total.Amount.Should().Be(2300m); // (2 * 1000) + (3 * 100)
    }
}