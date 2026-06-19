using Ecommerce.Application.DTOs.PaymentDtos;
using ECommerceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Mappings;

    public static class PaymentMappingExtensions
    {
        public static PaymentResponseDto ToDto(this Payment payment)
        {
            return new PaymentResponseDto
            {
                Id = payment.Id,
                Status = payment.Status.ToString(),
                Method = payment.Method.ToString(),
                Amount = payment.Amount.Amount,
                TransactionId = payment.TransactionId,
                CreatedAt = payment.CreatedAt,
                PaidAt = payment.PaidAt
            };
        }
    }

