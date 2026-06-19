
using Ecommerce.Application.DTOs;
using Ecommerce.Application.DTOs.OrderDtos;
using Ecommerce.Domain.Enums;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Common;
using FluentResults;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces;

public interface IOrderService
{
    Task<Result<OrderResponseDto>> CreateAsync(Guid userId, CreateOrderDto dto);
    Task<Result<OrderResponseDto>> GetByIdAsync(Guid id, Guid userId, bool isAdmin);

    Task<Result<PagedResult<OrderResponseDto>>> GetByUserIdAsync(
        Guid userId, PaginationParams pagination);

    Task<Result<PagedResult<OrderResponseDto>>> GetAllAsync(PaginationParams pagination);

    Task<Result> UpdateStatusAsync(Guid id, OrderStatus newStatus);
}