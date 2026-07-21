using Ecommerce.Application.DTOs.ReviewDtos;
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

public class ReviewServiceTests
{
    private readonly IReviewRepository _repository;
    private readonly ReviewService _service;

    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _productId = Guid.NewGuid();

    public ReviewServiceTests()
    {
        _repository = Substitute.For<IReviewRepository>();
        _service = new ReviewService(_repository);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFail_WhenUserHasNotPurchasedProduct()
    {
        _repository.UserHasPurchasedProductAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);

        var result = await _service.CreateAsync(_userId, _productId, new CreateReviewDto
        {
            Rating = 5,
            Comment = "Ótimo!"
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("purchased");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFail_WhenUserAlreadyReviewed()
    {
        _repository.UserHasPurchasedProductAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
        _repository.UserHasReviewedAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);

        var result = await _service.CreateAsync(_userId, _productId, new CreateReviewDto
        {
            Rating = 5,
            Comment = "Ótimo!"
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("already reviewed");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnOk_WhenValid()
    {
        _repository.UserHasPurchasedProductAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);
        _repository.UserHasReviewedAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(false);

        var result = await _service.CreateAsync(_userId, _productId, new CreateReviewDto
        {
            Rating = 5,
            Comment = "Ótimo produto!"
        });

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).AddAsync(Arg.Any<Review>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFail_WhenReviewNotFound()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.DeleteAsync(Guid.NewGuid(), _userId, false);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFail_WhenCustomerTriesToDeleteAnotherUsersReview()
    {
        var review = new Review(Guid.NewGuid(), _productId, new Rating(5), "Ok");
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns(review);

        var result = await _service.DeleteAsync(review.Id, _userId, isAdmin: false);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("permission");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnOk_WhenAdminDeletesAnyReview()
    {
        var review = new Review(Guid.NewGuid(), _productId, new Rating(5), "Ok");
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns(review);

        var result = await _service.DeleteAsync(review.Id, _userId, isAdmin: true);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(review);
    }
}