using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.ValueObjects
{


    public record Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "BRL")
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.");

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty.");

            Amount = amount;
            Currency = currency.ToUpper();
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("Cannot add different currencies.");
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Multiply(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative.");
            return new Money(Amount * quantity, Currency);
        }

        public override string ToString() => $"{Currency} {Amount:F2}";
    }
}
