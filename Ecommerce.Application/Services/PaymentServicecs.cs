using Ecommerce.Application.DTOs.PaymentDtos;
using Ecommerce.Application.Mappings;
using Ecommerce.Domain.Enums;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceApi.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;

    public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
    }

    public async Task<Result<PaymentResponseDto>> ProcessAsync(Guid orderId, CreatePaymentDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null)
            return Result.Fail("Order not found.");

        if (order.Status != OrderStatus.Pending)
            return Result.Fail("Order is not pending payment.");

        var payment = new Payment(orderId, order.Total, dto.Method);

        // simulação — 90% de chance de aprovar
        var approved = Random.Shared.Next(1, 11) <= 9;

        if (approved)
        {
            var transactionId = Guid.NewGuid().ToString();
            payment.Approve(transactionId);
            order.UpdateStatus(OrderStatus.Paid);
        }
        else
        {
            payment.Refuse();
        }

        await _paymentRepository.AddAsync(payment);
        await _orderRepository.UpdateAsync(order);

        return Result.Ok(payment.ToDto());
    }
}