using Ecommerce.Domain.Enums;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Exceptions;
using ECommerceApi.Domain.ValueObjects;
using EcommerceProject;
using FluentAssertions;
using Xunit;

namespace ECommerceApi.Tests.Domain;

public class PaymentTests
{
    private Payment CreatePayment() =>
        new Payment(Guid.NewGuid(), new Money(1000m), PaymentMethod.CreditCard);

    [Fact]
    public void Constructor_ShouldCreatePayment_WithPendingStatus()
    {
        var payment = CreatePayment();

        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.PaidAt.Should().BeNull();
        payment.TransactionId.Should().BeNull();
    }

    [Fact]
    public void Approve_ShouldApprovePayment_WhenPending()
    {
        var payment = CreatePayment();

        payment.Approve("TXN-123");

        payment.Status.Should().Be(PaymentStatus.Approved);
        payment.TransactionId.Should().Be("TXN-123");
        payment.PaidAt.Should().NotBeNull();
    }

    [Fact]
    public void Approve_ShouldThrow_WhenAlreadyApproved()
    {
        var payment = CreatePayment();
        payment.Approve("TXN-123");

        var act = () => payment.Approve("TXN-456");

        act.Should().Throw<InvalidPaymentStatusException>();
    }

    [Fact]
    public void Refuse_ShouldRefusePayment_WhenPending()
    {
        var payment = CreatePayment();

        payment.Refuse();

        payment.Status.Should().Be(PaymentStatus.Refused);
    }

    [Fact]
    public void Refuse_ShouldThrow_WhenNotPending()
    {
        var payment = CreatePayment();
        payment.Approve("TXN-123");

        var act = () => payment.Refuse();

        act.Should().Throw<InvalidPaymentStatusException>();
    }

    [Fact]
    public void Refund_ShouldRefundPayment_WhenApproved()
    {
        var payment = CreatePayment();
        payment.Approve("TXN-123");

        payment.Refund();

        payment.Status.Should().Be(PaymentStatus.Refunded);
    }

    [Fact]
    public void Refund_ShouldThrow_WhenNotApproved()
    {
        var payment = CreatePayment();

        var act = () => payment.Refund();

        act.Should().Throw<InvalidPaymentStatusException>();
    }
}