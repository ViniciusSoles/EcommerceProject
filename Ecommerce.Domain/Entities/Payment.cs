using Ecommerce.Domain.Enums;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.ValueObjects;
using EcommerceProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    public Order Order { get; private set; }

    protected Payment() { }

    public Payment(Guid orderId, Money amount, PaymentMethod method)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidPaymentStatusException("approve",Status.ToString());

        Status = PaymentStatus.Approved;
        TransactionId = transactionId;
        PaidAt = DateTime.UtcNow;
    }

    public void Refuse()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidPaymentStatusException("approve",Status.ToString());

        Status = PaymentStatus.Refused;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Approved)
            throw new InvalidPaymentStatusException("approve",Status.ToString());

        Status = PaymentStatus.Refunded;
    }
}
