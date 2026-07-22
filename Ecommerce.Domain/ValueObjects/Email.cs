using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.ValueObjects
{

    public record Email
    {
        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Campo Email não pode estar vazio.");
            if (!value.Contains("@"))
                throw new ArgumentException("E-mail com formato inválido");

            Value = value.ToLower().Trim();
        }

        public override string ToString() => Value;
    }
}
