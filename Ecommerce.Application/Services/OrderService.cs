using Ecommerce.Application.DTOs;
using Ecommerce.Application.DTOs.OrderDtos;
using Ecommerce.Application.DTOs.ProductDtos;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Enums;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Application.Mappings;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using ECommerceApi.Domain.ValueObjects;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceApi.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<OrderResponseDto>> CreateAsync(Guid userId, CreateOrderDto dto)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null || !cart.Items.Any())
            return Result.Fail("Cart is empty.");

        // valida estoque de todos os itens antes de criar o pedido
        foreach (var cartItem in cart.Items)
        {
            if (cartItem.Product.Stock < cartItem.Quantity)
                return Result.Fail($"Insufficient stock for {cartItem.Product.Name}.");
        }

        Address address;
        try
        {
            address = new Address(dto.Street, dto.Number, dto.City, dto.State, dto.ZipCode, dto.Complement);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message);
        }

        var orderItems = cart.Items.Select(ci => new OrderItem(
            Guid.Empty, // setado depois do Order existir
            ci.ProductId,
            ci.Product.Name,
            ci.Quantity,
            ci.UnitPrice)).ToList();

        Order order;
        try
        {
            order = new Order(userId, address, orderItems);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail(ex.Message);
        }

        // dá baixa no estoque
        foreach (var cartItem in cart.Items)
        {
            cartItem.Product.RemoveStock(cartItem.Quantity);
            await _productRepository.UpdateAsync(cartItem.Product);
        }

        await _orderRepository.AddAsync(order);

        // limpa o carrinho após criar o pedido
        cart.Clear();
        await _cartRepository.UpdateAsync(cart);

        return Result.Ok(order.ToDto());
    }

    public async Task<Result<OrderResponseDto>> GetByIdAsync(Guid id, Guid userId, bool isAdmin)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
            return Result.Fail("Order not found.");

        if (!isAdmin && order.UserId != userId)
            return Result.Fail("You don't have permission to view this order.");

        return Result.Ok(order.ToDto());
    }

    public async Task<Result<PagedResult<OrderResponseDto>>> GetByUserIdAsync(
        Guid userId, PaginationParams pagination)
    {
        var (items, total) = await _orderRepository.GetByUserIdAsync(userId, pagination);

        var result = new PagedResult<OrderResponseDto>
        {
            Data = items.ToDtoList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)pagination.PageSize)
        };

        return Result.Ok(result);
    }

    public async Task<Result<PagedResult<OrderResponseDto>>> GetAllAsync(PaginationParams pagination)
    {
        var (items, total) = await _orderRepository.GetAllAsync(pagination);

        var result = new PagedResult<OrderResponseDto>
        {
            Data = items.ToDtoList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)pagination.PageSize)
        };

        return Result.Ok(result);
    }

    public async Task<Result> UpdateStatusAsync(Guid id, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
            return Result.Fail("Order not found.");

        try
        {
            order.UpdateStatus(newStatus);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail(ex.Message);
        }

        await _orderRepository.UpdateAsync(order);

        return Result.Ok();
    }
}

