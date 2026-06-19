using Ecommerce.Application.DTOs;
using Ecommerce.Application.DTOs.ProductDtos;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.ValueObjects;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Application.Mappings;
using ECommerceApi.Domain.Common;
using ECommerceApi.Domain.Interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository repository, ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<PagedResult<ProductResponseDto>>> GetAllAsync(
        PaginationParams pagination,
        Guid? categoryId,
        string? searchTerm)
    {
        var (items, total) = await _repository.GetAllAsync(pagination, categoryId, searchTerm);

        var result = new PagedResult<ProductResponseDto>
        {
            Data = items.ToDtoList(),
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)pagination.PageSize)
        };

        return Result.Ok(result);
    }

    public async Task<Result<ProductResponseDto>> GetByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdWithReviewsAsync(id);

        if (product is null)
            return Result.Fail($"Product with id {id} not found.");

        return Result.Ok(product.ToDto());
    }

    public async Task<Result<ProductResponseDto>> CreateAsync(CreateProductDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);

        if (category is null)
            return Result.Fail("Category not found.");

        Product product;
        try
        {
            var price = new Money(dto.Price);
            product = new Product(dto.Name, dto.Description, price, dto.Stock, dto.CategoryId, dto.ImageUrl);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message);
        }

        await _repository.AddAsync(product);

        return Result.Ok(product.ToDto());
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product is null)
            return Result.Fail($"Product with id {id} not found.");

        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);

        if (category is null)
            return Result.Fail("Category not found.");

        try
        {
            var price = new Money(dto.Price);
            product.Update(dto.Name, dto.Description, price, dto.ImageUrl, dto.CategoryId);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message);
        }

        await _repository.UpdateAsync(product);

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product is null)
            return Result.Fail($"Product with id {id} not found.");

        await _repository.DeleteAsync(product);

        return Result.Ok();
    }

    public async Task<Result> DeactivateAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product is null)
            return Result.Fail($"Product with id {id} not found.");

        product.Deactivate();
        await _repository.UpdateAsync(product);

        return Result.Ok();
    }

    public async Task<Result> ActivateAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product is null)
            return Result.Fail($"Product with id {id} not found.");

        product.Activate();
        await _repository.UpdateAsync(product);

        return Result.Ok();
    }
}