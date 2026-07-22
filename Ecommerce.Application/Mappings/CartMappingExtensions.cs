using Ecommerce.Application.DTOs.CartDtos;
using ECommerceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Mappings;

public static class CartMappingExtensions
{
    public static CartItemResponseDto ToDto(this CartItem item)
    {
        return new CartItemResponseDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product?.Name ?? string.Empty,
            ImageUrl = item.Product?.ImageUrl,
            Quantity = item.Quantity,
            UnitPrice = item.Product.Price.Amount,
            Subtotal = item.GetSubtotal().Amount
        };
    }

    public static CartResponseDto ToDto(this Cart cart)
    {
        return new CartResponseDto
        {
            Id = cart.Id,
            Items = cart.Items.Select(i => i.ToDto()).ToList(),
            Total = cart.GetTotal().Amount,
            TotalItems = cart.Items.Sum(i => i.Quantity)
        };
    }
}