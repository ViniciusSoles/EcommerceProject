using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Entities;

public class Cart
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public User User { get; private set; }
    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

    protected Cart() { }

    public Cart(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddItem(CartItem item)
    {
        var existing = Items.FirstOrDefault(i => i.ProductId == item.ProductId);

        if (existing is not null)
            existing.UpdateQuantity(existing.Quantity + item.Quantity);
        else
            Items.Add(item);

        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(Guid cartItemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item is null)
            throw new InvalidOperationException("Item not found in cart.");

        Items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Clear()
    {
        Items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public Money GetTotal() =>
        Items.Aggregate(
            new Money(0),
            (total, item) => total.Add(item.GetSubtotal()));
}