using Ecommerce.Application.DTOs;
using Ecommerce.Application.DTOs.CategoryDtos;
using Ecommerce.Domain.Entities;
using ECommerceApi.Application.Interfaces;
using ECommerceApi.Application.Mappings;
using ECommerceApi.Domain.Interfaces;
using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApi.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<CategoryResponseDto>>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return Result.Ok(categories.ToDtoList());
    }

    public async Task<Result<CategoryResponseDto>> GetByIdAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category is null)
            return Result.Fail(
                new Error("Categoria não encontrada.").WithMetadata("ErrorCode", "CATEGORY_NOT_FOUND"));

        return Result.Ok(category.ToDto());
    }

    public async Task<Result<CategoryResponseDto>> CreateAsync(CreateCategoryDto dto)
    {
        if (await _repository.NameExistsAsync(dto.Name))
            return Result.Fail(
                new Error("Uma categoria com este nome já existe.").WithMetadata("ErrorCode", "CATEGORY_ALREADY_EXISTS"));

        Category category;
        try
        {
            category = new Category(dto.Name, dto.Description);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(
                new Error(ex.Message).WithMetadata("ErrorCode", "INVALID_CATEGORY"));
        }

        await _repository.AddAsync(category);

        return Result.Ok(category.ToDto());
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category is null)
            return Result.Fail(
                new Error($"Category with id {id} not found.").WithMetadata("ErrorCode", "CATEGORY_NOT_FOUND"));

        try
        {
            category.Update(dto.Name, dto.Description);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(
                new Error(ex.Message).WithMetadata("ErrorCode", "INVALID_CATEGORY"));
        }

        await _repository.UpdateAsync(category);

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category is null)
            return Result.Fail(
                new Error($"Category with id {id} not found.").WithMetadata("ErrorCode", "CATEGORY_NOT_FOUND"));

        if (await _repository.HasProductsAsync(id))
            return Result.Fail(
                new Error("Cannot delete a category that has products.").WithMetadata("ErrorCode", "CATEGORY_HAS_PRODUCTS"));

        await _repository.DeleteAsync(category);

        return Result.Ok();
    }
}
