using Ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.PaymentDtos
{
    public class CreatePaymentDto
    {
        public PaymentMethod Method { get; set; }
    }
}
