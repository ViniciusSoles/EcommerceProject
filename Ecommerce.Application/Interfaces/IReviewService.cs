using Ecommerce.Application.DTOs.ReviewDtos;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces;

public interface IReviewService
{
    Task<Result<IEnumerable<ReviewResponseDto>>> GetByProductIdAsync(Guid productId);
    Task<Result<ReviewResponseDto>> CreateAsync(Guid userId, Guid productId, CreateReviewDto dto);
    Task<Result> DeleteAsync(Guid reviewId, Guid userId, bool isAdmin);
}
