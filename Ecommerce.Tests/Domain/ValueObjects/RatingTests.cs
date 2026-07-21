using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.ValueObjects;
using FluentAssertions;
using Xunit;
using TheoryAttribute = NUnit.Framework.TheoryAttribute;

namespace Ecommerce.Tests.Domain.ValueObjects;

public class RatingTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Constructor_ShouldCreateRating_WhenValid(int value)
    {
        var rating = new Rating(value);

        rating.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Constructor_ShouldThrow_WhenOutOfRange(int value)
    {
        var act = () => new Rating(value);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Rating must be between 1 and 5*");
    }
}