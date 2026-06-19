using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.PaymentDtos
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
