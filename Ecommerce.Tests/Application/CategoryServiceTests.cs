using Ecommerce.Application.DTOs;
using Ecommerce.Domain.Entities;
using ECommerceApi.Application.Services;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace ECommerceApi.Tests.Application;

public class CategoryServiceTests
{
    private readonly ICategoryRepository _repository;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _repository = Substitute.For<ICategoryRepository>();
        _service = new CategoryService(_repository);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategories()
    {
        var categories = new List<Category>
        {
            new Category("Eletrônicos"),
            new Category("Roupas")
        };

        _repository.GetAllAsync().Returns(categories);

        var result = await _service.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFail_WhenNotFound()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("not found");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFail_WhenNameAlreadyExists()
    {
        _repository.NameExistsAsync(Arg.Any<string>()).Returns(true);

        var result = await _service.CreateAsync(new CreateCategoryDto
        {
            Name = "Eletrônicos"
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnOk_WhenValid()
    {
        _repository.NameExistsAsync(Arg.Any<string>()).Returns(false);

        var result = await _service.CreateAsync(new CreateCategoryDto
        {
            Name = "Eletrônicos",
            Description = "Produtos eletrônicos"
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Eletrônicos");
        await _repository.Received(1).AddAsync(Arg.Any<Category>());
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFail_WhenCategoryHasProducts()
    {
        var category = new Category("Eletrônicos");
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns(category);
        _repository.HasProductsAsync(Arg.Any<Guid>()).Returns(true);

        var result = await _service.DeleteAsync(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("has products");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnOk_WhenValid()
    {
        var category = new Category("Eletrônicos");
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns(category);
        _repository.HasProductsAsync(Arg.Any<Guid>()).Returns(false);

        var result = await _service.DeleteAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(Arg.Any<Category>());
    }
}