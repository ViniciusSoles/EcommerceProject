using Ecommerce.Domain.Entities;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.ValueObjects;

namespace ECommerceApi.Domain.Entities;

public class Review
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ProductId { get; private set; }
    public Rating Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; }
    public Product Product { get; private set; }

    protected Review() { }

    public Review(Guid userId, Guid productId, Rating rating, string? comment = null)
    {
        if (comment?.Length > 500)
            throw new ArgumentException("Comentário excedeu o limite 500 caracteres.");

        Id = Guid.NewGuid();
        UserId = userId;
        ProductId = productId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(Rating rating, string? comment)
    {
        if (comment?.Length > 500)
            throw new ArgumentException("Comentário excedeu o limite de 500 caracteres");

        Rating = rating;
        Comment = comment;
    }
}