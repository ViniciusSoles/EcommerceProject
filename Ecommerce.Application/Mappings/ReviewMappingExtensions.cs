using Ecommerce.Application.DTOs.ReviewDtos;
using ECommerceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Mappings;

public static class ReviewMappingExtensions
{
    public static ReviewResponseDto ToDto(this Review review)
    {
        return new ReviewResponseDto
        {
            Id = review.Id,
            UserId = review.UserId,
            UserName = review.User?.Name ?? string.Empty,
            Rating = review.Rating.Value,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }

    public static IEnumerable<ReviewResponseDto> ToDtoList(this IEnumerable<Review> reviews)
    {
        return reviews.Select(r => r.ToDto());
    }
}