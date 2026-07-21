using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ecommerce.Tests.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_ShouldCreateMoney_WhenValid()
    {
        var money = new Money(100m);

        money.Amount.Should().Be(100m);
        money.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenAmountIsNegative()
    {
        var act = () => new Money(-1m);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Amount cannot be negative*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCurrencyIsEmpty()
    {
        var act = () => new Money(100m, "");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Currency cannot be empty*");
    }

    [Fact]
    public void Add_ShouldReturnSum_WhenSameCurrency()
    {
        var money1 = new Money(100m);
        var money2 = new Money(50m);

        var result = money1.Add(money2);

        result.Amount.Should().Be(150m);
    }

    [Fact]
    public void Add_ShouldThrow_WhenDifferentCurrencies()
    {
        var brl = new Money(100m, "BRL");
        var usd = new Money(100m, "USD");

        var act = () => brl.Add(usd);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot add different currencies*");
    }

    [Fact]
    public void Multiply_ShouldReturnCorrectValue()
    {
        var money = new Money(100m);

        var result = money.Multiply(3);

        result.Amount.Should().Be(300m);
    }

    [Fact]
    public void Multiply_ShouldThrow_WhenQuantityIsNegative()
    {
        var money = new Money(100m);

        var act = () => money.Multiply(-1);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Quantity cannot be negative*");
    }

    [Fact]
    public void TwoMoneys_ShouldBeEqual_WhenSameAmountAndCurrency()
    {
        var money1 = new Money(100m, "BRL");
        var money2 = new Money(100m, "BRL");

        money1.Should().Be(money2);
    }
}