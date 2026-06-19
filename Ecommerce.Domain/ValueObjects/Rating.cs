using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.ValueObjects
{
   

    public record Rating
    {
        public int Value { get; }

        public Rating(int value)
        {
            if (value < 1 || value > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");
            Value = value;
        }

        public override string ToString() => $"{Value}/5";
    }
}
