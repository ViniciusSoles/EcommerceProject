using ECommerceApi.Domain.ValueObjects;
using FluentAssertions;
using Xunit;
using TheoryAttribute = NUnit.Framework.TheoryAttribute;

namespace Ecommerce.Tests.Domain.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Constructor_ShouldCreateAddress_WhenValid()
    {
        var address = new Address("Rua A", "123", "São Paulo", "SP", "01000-000");

        address.Street.Should().Be("Rua A");
        address.Number.Should().Be("123");
        address.ZipCode.Should().Be("01000000"); // ← hífen removido no construtor
    }

    [Theory]
    [InlineData("", "123", "São Paulo", "SP", "01000-000")]
    [InlineData("Rua A", "", "São Paulo", "SP", "01000-000")]
    [InlineData("Rua A", "123", "", "SP", "01000-000")]
    [InlineData("Rua A", "123", "São Paulo", "", "01000-000")]
    [InlineData("Rua A", "123", "São Paulo", "SP", "")]
    public void Constructor_ShouldThrow_WhenAnyRequiredFieldIsEmpty(
        string street, string number, string city, string state, string zipCode)
    {
        var act = () => new Address(street, number, city, state, zipCode);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ShouldRemoveHyphen_FromZipCode()
    {
        var address = new Address("Rua A", "123", "São Paulo", "SP", "01000-000");

        address.ZipCode.Should().Be("01000000");
    }
}