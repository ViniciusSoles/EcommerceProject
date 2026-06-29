using Ecommerce.Application.DTOs.CartDtos;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Application.Mappings;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<CartResponseDto>> GetByUserIdAsync(Guid userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return Result.Ok(cart.ToDto());
    }

    public async Task<Result<CartResponseDto>> AddItemAsync(Guid userId, AddCartItemDto dto)
    {
        var product = await _productRepository.GetByIdAsync(dto.ProductId);

        if (product is null)
            return Result.Fail("Product not found.");

        if (!product.IsActive)
            return Result.Fail("Product is not available.");

        if (product.Stock < dto.Quantity)
            return Result.Fail("Insufficient stock.");

        var cart = await GetOrCreateCartAsync(userId);

        var item = new CartItem(cart.Id, product.Id, dto.Quantity);
        cart.AddItem(item);

        await _cartRepository.UpdateAsync(cart);

        return Result.Ok(cart.ToDto());
    }

    public async Task<Result<CartResponseDto>> UpdateItemAsync(
        Guid userId, Guid cartItemId, UpdateCartItemDto dto)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
            return Result.Fail("Cart not found.");

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);

        if (item is null)
            return Result.Fail("Item not found in cart.");

        if (item.Product.Stock < dto.Quantity)
            return Result.Fail("Insufficient stock.");

        try
        {
            item.UpdateQuantity(dto.Quantity);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message);
        }

        await _cartRepository.UpdateAsync(cart);

        return Result.Ok(cart.ToDto());
    }

    public async Task<Result> RemoveItemAsync(Guid userId, Guid cartItemId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
            return Result.Fail("Cart not found.");

        try
        {
            cart.RemoveItem(cartItemId);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Fail(ex.Message);
        }

        await _cartRepository.UpdateAsync(cart);

        return Result.Ok();
    }

    public async Task<Result> ClearAsync(Guid userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
            return Result.Fail("Cart not found.");

        cart.Clear();
        await _cartRepository.UpdateAsync(cart);

        return Result.Ok();
    }

    private async Task<Cart> GetOrCreateCartAsync(Guid userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
        {
            cart = new Cart(userId);
            await _cartRepository.AddAsync(cart);
        }

        return cart;
    }
}

