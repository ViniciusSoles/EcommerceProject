using Ecommerce.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using ECommerceApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(Guid id) =>
        await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Category>> GetAllAsync() =>
        await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<bool> NameExistsAsync(string name) =>
        await _context.Categories.AnyAsync(c => c.Name == name);

    public async Task<bool> HasProductsAsync(Guid categoryId) =>
        await _context.Products.AnyAsync(p => p.CategoryId == categoryId);

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}