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
            return Result.Fail(
                new Error("You can only review products you have purchased.").WithMetadata("ErrorCode", "USER_NOT_PURCHASER"));

        if (await _repository.UserHasReviewedAsync(userId, productId))
            return Result.Fail(
                new Error("You have already reviewed this product.").WithMetadata("ErrorCode", "USER_ALREADY_REVIEWED"));

        Review review;
        try
        {
            var rating = new Rating(dto.Rating);
            review = new Review(userId, productId, rating, dto.Comment);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(
                new Error(ex.Message).WithMetadata("ErrorCode", "INVALID_REVIEW"));
        }

        await _repository.AddAsync(review);

        return Result.Ok(review.ToDto());
    }

    public async Task<Result> DeleteAsync(Guid reviewId, Guid userId, bool isAdmin)
    {
        var review = await _repository.GetByIdAsync(reviewId);

        if (review is null)
            return Result.Fail(
                new Error("Review not found.").WithMetadata("ErrorCode", "REVIEW_NOT_FOUND"));

        if (!isAdmin && review.UserId != userId)
            return Result.Fail(
                new Error("You don't have permission to delete this review.").WithMetadata("ErrorCode", "USER_NOT_ALLOWED"));

        await _repository.DeleteAsync(review);

        return Result.Ok();
    }
}