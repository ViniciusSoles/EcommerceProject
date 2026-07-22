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
            return Result.Fail("Categoria não encontrada.");

        return Result.Ok(category.ToDto());
    }

    public async Task<Result<CategoryResponseDto>> CreateAsync(CreateCategoryDto dto)
    {
        if (await _repository.NameExistsAsync(dto.Name))
            return Result.Fail("Uma categoria com este nome já existe.");

        Category category;
        try
        {
            category = new Category(dto.Name, dto.Description);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message);
        }

        await _repository.AddAsync(category);

        return Result.Ok(category.ToDto());
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category is null)
            return Result.Fail($"Category with id {id} not found.");

        try
        {
            category.Update(dto.Name, dto.Description);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(ex.Message);
        }

        await _repository.UpdateAsync(category);

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category is null)
            return Result.Fail($"Category with id {id} not found.");

        if (await _repository.HasProductsAsync(id))
            return Result.Fail("Cannot delete a category that has products.");

        await _repository.DeleteAsync(category);

        return Result.Ok();
    }
}
