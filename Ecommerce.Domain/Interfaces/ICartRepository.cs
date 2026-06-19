using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Domain.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId);
    Task<Cart?> GetByIdWithItemsAsync(Guid cartId);
    Task AddAsync(Cart cart);
    Task UpdateAsync(Cart cart);
}