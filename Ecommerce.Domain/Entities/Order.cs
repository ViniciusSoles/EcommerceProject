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

public class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money Total { get; private set; }
    public Address ShippingAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public User User { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
    public Payment? Payment { get; private set; }

    protected Order() { }

    public Order(Guid userId, Address shippingAddress, IEnumerable<OrderItem> items)
    {
        if (!items.Any())
            throw new InvalidOperationException("Order must have at least one item.");

        Id = Guid.NewGuid();
        UserId = userId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        foreach (var item in items)
            Items.Add(item);

        Total = CalculateTotal();
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        ValidateStatusTransition(newStatus);
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new CannotCancelShippedOrderException();

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    private Money CalculateTotal() =>
        Items.Aggregate(
            new Money(0),
            (total, item) => total.Add(item.GetSubtotal()));

    private void ValidateStatusTransition(OrderStatus newStatus)
    {
        var valid = (Status, newStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Paid) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Paid, OrderStatus.Processing) => true,
            (OrderStatus.Paid, OrderStatus.Cancelled) => true,
            (OrderStatus.Processing, OrderStatus.Shipped) => true,
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            _ => false
        };

        if (!valid)
            throw new InvalidOrderStatusException(Status.ToString(),newStatus.ToString());
            
    }
}