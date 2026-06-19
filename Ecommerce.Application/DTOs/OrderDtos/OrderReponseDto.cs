using Ecommerce.Application.DTOs.PaymentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.OrderDtos
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItemResponseDto> Items { get; set; }
        public PaymentResponseDto? Payment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
