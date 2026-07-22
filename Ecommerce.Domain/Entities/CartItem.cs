using Ecommerce.Domain.Entities;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.Entities;

public class CartItem
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Cart Cart { get; private set; }
    public Product Product { get; private set; }

    protected CartItem() { }

    public CartItem(Guid cartId, Guid productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser positiva.");

        Id = Guid.NewGuid();
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser positiva.");
        Quantity = quantity;
    }

    public Money GetSubtotal() => Product.Price.Multiply(Quantity);
}
