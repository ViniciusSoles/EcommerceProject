using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Domain.ValueObjects;

public record Address
{
    public string Street { get; }
    public string Number { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string? Complement { get; }

    public Address(
        string street,
        string number,
        string city,
        string state,
        string zipCode,
        string? complement = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty.");
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Number cannot be empty.");
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.");
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be empty.");
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode cannot be empty.");

        Street = street;
        Number = number;
        City = city;
        State = state;
        ZipCode = zipCode.Replace("-", "").Trim();
        Complement = complement;
    }

    public override string ToString() =>
        $"{Street}, {Number}{(Complement != null ? $" - {Complement}" : "")}, {City} - {State}, {ZipCode}";
}