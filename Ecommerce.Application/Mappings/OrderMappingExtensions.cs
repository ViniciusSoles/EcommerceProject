using Ecommerce.Application.DTOs.OrderDtos;
using Ecommerce.Application.Mappings;
using ECommerceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Mappings;

public static class OrderMappingExtensions
{
    public static OrderItemResponseDto ToDto(this OrderItem item)
    {
        return new OrderItemResponseDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice.Amount,
            Subtotal = item.GetSubtotal().Amount
        };
    }

    public static OrderResponseDto ToDto(this Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            Status = order.Status.ToString(),
            Total = order.Total.Amount,
            ShippingAddress = order.ShippingAddress.ToString(),
            Items = order.Items.Select(i => i.ToDto()).ToList(),
            Payment = order.Payment?.ToDto(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }

    public static IEnumerable<OrderResponseDto> ToDtoList(this IEnumerable<Order> orders)
    {
        return orders.Select(o => o.ToDto());
    }
}