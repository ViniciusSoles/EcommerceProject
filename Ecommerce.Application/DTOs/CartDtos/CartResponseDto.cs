using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.CartDtos
{
    public class CartResponseDto
    {
        public Guid Id { get; set; }
        public List<CartItemResponseDto> Items { get; set; }
        public decimal Total { get; set; }
        public int TotalItems { get; set; }
    }
}
