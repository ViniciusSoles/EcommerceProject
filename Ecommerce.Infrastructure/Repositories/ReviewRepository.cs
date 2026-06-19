using Ecommerce.Domain.Enums;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using ECommerceApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(Guid id) =>
        await _context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId) =>
        await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<bool> UserHasReviewedAsync(Guid userId, Guid productId) =>
        await _context.Reviews
            .AnyAsync(r => r.UserId == userId && r.ProductId == productId);

    public async Task<bool> UserHasPurchasedProductAsync(Guid userId, Guid productId) =>
        await _context.Orders
            .Where(o => o.UserId == userId &&
                       (o.Status == OrderStatus.Delivered ||
                        o.Status == OrderStatus.Shipped))
            .SelectMany(o => o.Items)
            .AnyAsync(i => i.ProductId == productId);

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Review review)
    {
        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
    }
}
