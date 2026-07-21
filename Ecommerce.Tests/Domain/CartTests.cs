using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ECommerceApi.Tests.Domain;

public class CartTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();

    private CartItem CreateCartItem(Guid? productId = null, int quantity = 1) =>
        new CartItem(Guid.NewGuid(), productId ?? _productId, quantity);

    [Fact]
    public void Constructor_ShouldCreateCart_WhenValid()
    {
        var cart = new Cart(_userId);

        cart.UserId.Should().Be(_userId);
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_ShouldAddItem_WhenProductNotInCart()
    {
        var cart = new Cart(_userId);
        var item = CreateCartItem();

        cart.AddItem(item);

        cart.Items.Should().HaveCount(1);
    }

    [Fact]
    public void AddItem_ShouldMergeQuantity_WhenProductAlreadyInCart()
    {
        var cart = new Cart(_userId);
        var item1 = CreateCartItem(_productId, 2);
        var item2 = CreateCartItem(_productId, 3);

        cart.AddItem(item1);
        cart.AddItem(item2);

        cart.Items.Should().HaveCount(1);
        cart.Items.First().Quantity.Should().Be(5);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItem_WhenExists()
    {
        var cart = new Cart(_userId);
        var item = CreateCartItem();
        cart.AddItem(item);

        cart.RemoveItem(item.Id);

        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_ShouldThrow_WhenItemNotFound()
    {
        var cart = new Cart(_userId);

        var act = () => cart.RemoveItem(Guid.NewGuid());

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Item not found in cart*");
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var cart = new Cart(_userId);
        cart.AddItem(CreateCartItem(Guid.NewGuid()));
        cart.AddItem(CreateCartItem(Guid.NewGuid()));

        cart.Clear();

        cart.Items.Should().BeEmpty();
    }
}