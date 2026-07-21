using Ecommerce.Application.DTOs.ProductDtos;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Application.Services;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Entities;
using ECommerceApi.Domain.Interfaces;
using ECommerceApi.Domain.ValueObjects;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace ECommerceApi.Tests.Application;

public class ProductServiceTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ProductService _service;

    private readonly Guid _categoryId = Guid.NewGuid();

    public ProductServiceTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _service = new ProductService(_productRepository, _categoryRepository);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFail_WhenNotFound()
    {
        _productRepository.GetByIdWithReviewsAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFail_WhenCategoryNotFound()
    {
        _categoryRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.CreateAsync(new CreateProductDto
        {
            Name = "Notebook",
            Description = "Descrição",
            Price = 1000m,
            Stock = 10,
            CategoryId = _categoryId
        });

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Category not found");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnOk_WhenValid()
    {
        var category = new Category("Eletrônicos");
        _categoryRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(category);

        var result = await _service.CreateAsync(new CreateProductDto
        {
            Name = "Notebook",
            Description = "Descrição",
            Price = 1000m,
            Stock = 10,
            CategoryId = _categoryId
        });

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Notebook");
        await _productRepository.Received(1).AddAsync(Arg.Any<Product>());
    }

    [Fact]
    public async Task DeactivateAsync_ShouldReturnFail_WhenNotFound()
    {
        _productRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        var result = await _service.DeactivateAsync(Guid.NewGuid());

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateAsync_ShouldDeactivate_WhenValid()
    {
        var product = new Product("Notebook", "Descrição", new Money(1000m), 10, _categoryId);
        _productRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(product);

        var result = await _service.DeactivateAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        product.IsActive.Should().BeFalse();
        await _productRepository.Received(1).UpdateAsync(product);
    }
}