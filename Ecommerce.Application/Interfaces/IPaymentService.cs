using Ecommerce.Application.DTOs.PaymentDtos;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentResponseDto>> ProcessAsync(Guid orderId, CreatePaymentDto dto);
}