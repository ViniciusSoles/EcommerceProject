using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ECommerceApi.Tests.Domain;

public class ReviewTests
{
    [Fact]
    public void Constructor_ShouldCreateReview_WhenValid()
    {
        var review = new Review(Guid.NewGuid(), Guid.NewGuid(), new Rating(5), "Ótimo produto!");

        review.Rating.Value.Should().Be(5);
        review.Comment.Should().Be("Ótimo produto!");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCommentExceeds500Chars()
    {
        var longComment = new string('A', 501);

        var act = () => new Review(Guid.NewGuid(), Guid.NewGuid(), new Rating(5), longComment);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Comment cannot exceed 500 characters*");
    }

    [Fact]
    public void Update_ShouldUpdateRatingAndComment()
    {
        var review = new Review(Guid.NewGuid(), Guid.NewGuid(), new Rating(3), "Ok");

        review.Update(new Rating(5), "Excelente!");

        review.Rating.Value.Should().Be(5);
        review.Comment.Should().Be("Excelente!");
    }

    [Fact]
    public void Constructor_ShouldAllowNullComment()
    {
        var review = new Review(Guid.NewGuid(), Guid.NewGuid(), new Rating(4), null);

        review.Comment.Should().BeNull();
    }
}