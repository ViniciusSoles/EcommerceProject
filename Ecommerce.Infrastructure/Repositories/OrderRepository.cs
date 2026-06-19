using ECommerceApi.Domain.Common;
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

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id) =>
        await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<(IEnumerable<Order> Items, int Total)> GetByUserIdAsync(
        Guid userId, PaginationParams pagination)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Payment)
            .Where(o => o.UserId == userId)
            .AsQueryable();

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<(IEnumerable<Order> Items, int Total)> GetAllAsync(
        PaginationParams pagination)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Payment)
            .AsQueryable();

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
