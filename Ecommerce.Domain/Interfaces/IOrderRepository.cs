using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);

    Task<(IEnumerable<Order> Items, int Total)> GetByUserIdAsync(
        Guid userId,
        PaginationParams pagination);

    Task<(IEnumerable<Order> Items, int Total)> GetAllAsync(
        PaginationParams pagination);

    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}