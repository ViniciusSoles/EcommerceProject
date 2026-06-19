using Ecommerce.Application.DTOs.CategoryDtos;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Mappings;

public static class CategoryMappingExtensions
{
    public static CategoryResponseDto ToDto(this Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt
        };
    }

    public static IEnumerable<CategoryResponseDto> ToDtoList(this IEnumerable<Category> categories)
    {
        return categories.Select(c => c.ToDto());
    }
}