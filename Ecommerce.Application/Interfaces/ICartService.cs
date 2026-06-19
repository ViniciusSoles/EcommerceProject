using Ecommerce.Application.DTOs.CartDtos;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces;

public interface ICartService
{
    Task<Result<CartResponseDto>> GetByUserIdAsync(Guid userId);
    Task<Result<CartResponseDto>> AddItemAsync(Guid userId, AddCartItemDto dto);
    Task<Result<CartResponseDto>> UpdateItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto dto);
    Task<Result> RemoveItemAsync(Guid userId, Guid cartItemId);
    Task<Result> ClearAsync(Guid userId);
}
