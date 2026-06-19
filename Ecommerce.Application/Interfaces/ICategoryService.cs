using Ecommerce.Application.DTOs;
using Ecommerce.Application.DTOs.CategoryDtos;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryResponseDto>>> GetAllAsync();
    Task<Result<CategoryResponseDto>> GetByIdAsync(Guid id);
    Task<Result<CategoryResponseDto>> CreateAsync(CreateCategoryDto dto);
    Task<Result> UpdateAsync(Guid id, UpdateCategoryDto dto);
    Task<Result> DeleteAsync(Guid id);
}