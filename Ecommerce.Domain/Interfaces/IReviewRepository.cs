using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Domain.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id);
    Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId);
    Task<bool> UserHasReviewedAsync(Guid userId, Guid productId);
    Task<bool> UserHasPurchasedProductAsync(Guid userId, Guid productId);
    Task AddAsync(Review review);
    Task UpdateAsync(Review review);
    Task DeleteAsync(Review review);
}