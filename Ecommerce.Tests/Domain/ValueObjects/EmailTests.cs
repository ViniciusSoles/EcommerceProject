using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Domain.ValueObjects;
using FluentAssertions;
using Xunit;
using TheoryAttribute = NUnit.Framework.TheoryAttribute;

namespace Ecommerce.Tests.Domain.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Constructor_ShouldCreateEmail_WhenValid()
    {
        var email = new Email("Mock@Gmail.COM");

        email.Value.Should().Be("mock@gmail.com");
    }

    [Fact]
    public void Constructor_ShouldNormalize_ToLowerCase()
    {
        var email = new Email("UPPER@GMAIL.COM");

        email.Value.Should().Be("upper@gmail.com");
    }

    [Fact]
    public void Constructor_ShouldTrim_WhiteSpaces()
    {
        var email = new Email("  mock@gmail.com  ");

        email.Value.Should().Be("mock@gmail.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_ShouldThrow_WhenEmpty(string value)
    {
        var act = () => new Email(value);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Email cannot be empty*");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenInvalidFormat()
    {
        var act = () => new Email("invalidemail");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid email format*");
    }

    [Fact]
    public void TwoEmails_ShouldBeEqual_WhenSameValue()
    {
        var email1 = new Email("mock@gmail.com");
        var email2 = new Email("mock@gmail.com");

        email1.Should().Be(email2);
    }
}