using Ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.OrderDtos
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
