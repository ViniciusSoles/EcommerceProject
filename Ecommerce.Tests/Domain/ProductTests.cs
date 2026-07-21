using Ecommerce.Domain.Entities;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Exceptions;
using ECommerceApi.Domain.ValueObjects;
using FluentAssertions;
using Xunit;
using TheoryAttribute = NUnit.Framework.TheoryAttribute;

namespace ECommerceApi.Tests.Domain;

public class ProductTests
{
    private readonly Guid _categoryId = Guid.NewGuid();

    private Product CreateProduct(string name = "Notebook", int stock = 10) =>
        new Product(name, "Descrição", new Money(1000m), stock, _categoryId);

    [Fact]
    public void Constructor_ShouldCreateProduct_WhenValid()
    {
        var product = CreateProduct();

        product.Name.Should().Be("Notebook");
        product.Stock.Should().Be(10);
        product.IsActive.Should().BeTrue();
        product.Price.Amount.Should().Be(1000m);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrow_WhenNameIsEmpty(string name)
    {
        var act = () => CreateProduct(name);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Name cannot be empty*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenStockIsNegative()
    {
        var act = () => CreateProduct(stock: -1);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Stock cannot be negative*");
    }

    [Fact]
    public void AddStock_ShouldIncreaseStock()
    {
        var product = CreateProduct(stock: 10);

        product.AddStock(5);

        product.Stock.Should().Be(15);
    }

    [Fact]
    public void AddStock_ShouldThrow_WhenQuantityIsZeroOrNegative()
    {
        var product = CreateProduct();

        var act = () => product.AddStock(0);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Quantity must be positive*");
    }

    [Fact]
    public void RemoveStock_ShouldDecreaseStock()
    {
        var product = CreateProduct(stock: 10);

        product.RemoveStock(3);

        product.Stock.Should().Be(7);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var product = CreateProduct();

        product.Deactivate();

        product.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        var product = CreateProduct();
        product.Deactivate();

        product.Activate();

        product.IsActive.Should().BeTrue();
    }
}