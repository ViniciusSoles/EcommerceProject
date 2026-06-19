using Ecommerce.Application.DTOs.ProductDtos;
using Ecommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Mappings;

public static class ProductMappingExtensions
{
    public static ProductResponseDto ToDto(this Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            AverageRating = product.GetAverageRating(),
            CreatedAt = product.CreatedAt
        };
    }

    public static IEnumerable<ProductResponseDto> ToDtoList(this IEnumerable<Product> products)
    {
        return products.Select(p => p.ToDto());
    }
}