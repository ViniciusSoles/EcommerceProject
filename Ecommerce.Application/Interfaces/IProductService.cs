using Ecommerce.Application.DTOs;
using Ecommerce.Application.DTOs.ProductDtos;
using ECommerceApi.Domain.Common;
using FluentResults;

namespace ECommerceApi.Application.Interfaces;

public interface IProductService
{
    Task<Result<PagedResult<ProductResponseDto>>> GetAllAsync(
        PaginationParams pagination,
        Guid? categoryId,
        string? searchTerm);

    Task<Result<ProductResponseDto>> GetByIdAsync(Guid id);
    Task<Result<ProductResponseDto>> CreateAsync(CreateProductDto dto);
    Task<Result> UpdateAsync(Guid id, UpdateProductDto dto);
    Task<Result> DeleteAsync(Guid id);
    Task<Result> DeactivateAsync(Guid id);
    Task<Result> ActivateAsync(Guid id);
}