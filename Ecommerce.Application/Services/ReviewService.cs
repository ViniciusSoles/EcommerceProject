using Ecommerce.Application.DTOs.ReviewDtos;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Application.Mappings;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repository;

    public ReviewService(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<ReviewResponseDto>>> GetByProductIdAsync(Guid productId)
    {
        var reviews = await _repository.GetByProductIdAsync(productId);
        return Result.Ok(reviews.ToDtoList());
    }

    public async Task<Result<ReviewResponseDto>> CreateAsync(
        Guid userId, Guid productId, CreateReviewDto dto)
    {
        if (!await _repository.UserHasPurchasedProductAsync(userId, productId))
            return Result.Fail("You can only review products you have purchased.");

        if (await _repository.UserHasReviewedAsync(userId, productId))
            return Result.Fail("You have already reviewed this product.");

        Review review;
        try
        {
            var rating = new Rating(dto.Rating);
            review = new Review(userId, productId, rating, dto.Comment);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message);
        }

        await _repository.AddAsync(review);

        return Result.Ok(review.ToDto());
    }

    public async Task<Result> DeleteAsync(Guid reviewId, Guid userId, bool isAdmin)
    {
        var review = await _repository.GetByIdAsync(reviewId);

        if (review is null)
            return Result.Fail("Review not found.");

        if (!isAdmin && review.UserId != userId)
            return Result.Fail("You don't have permission to delete this review.");

        await _repository.DeleteAsync(review);

        return Result.Ok();
    }
}