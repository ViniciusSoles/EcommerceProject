using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Entities
{
 
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Money Price { get; private set; }
        public int Stock { get; private set; }
        public string? ImageUrl { get; private set; }
        public bool IsActive { get; private set; }
        public Guid CategoryId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // nav props
        public Category Category { get; private set; }
        public ICollection<Review> Reviews { get; private set; } = new List<Review>();
        public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; private set; } = new List<CartItem>();

        protected Product() { }

        public Product(
            string name,
            string description,
            Money price,
            int stock,
            Guid categoryId,
            string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.");
            if (stock < 0)
                throw new ArgumentException("Stock cannot be negative.");

            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            CategoryId = categoryId;
            ImageUrl = imageUrl;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string name,
            string description,
            Money price,
            string? imageUrl,
            Guid categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.");

            Name = name;
            Description = description;
            Price = price;
            ImageUrl = imageUrl;
            CategoryId = categoryId;
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive.");
            Stock += quantity;
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive.");
            if (quantity > Stock)
                throw new InvalidOperationException("Insufficient stock.");
            Stock -= quantity;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;

        public double GetAverageRating() =>
            Reviews.Any()
                ? Reviews.Average(r => r.Rating.Value)
                : 0;
    }
}
