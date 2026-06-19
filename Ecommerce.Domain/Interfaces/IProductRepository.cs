using Ecommerce.Domain.Entities;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetByIdWithReviewsAsync(Guid id);

    Task<(IEnumerable<Product> Items, int Total)> GetAllAsync(
        PaginationParams pagination,
        Guid? categoryId,
        string? searchTerm);

    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}