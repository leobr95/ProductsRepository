using System;
using RealEstate.Application.DTOs;
using RealEstate.Application.Interfaces;
using RealEstate.Application.Validation;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;

namespace RealEstate.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly IValidationService _validator;

    public ProductService(IProductRepository repo, IValidationService validator)
    {
        _repo = repo;
        _validator = validator;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(string? q, CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(q, ct);
        return items.Select(Map);
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e is null ? null : Map(e);
    }

    public async Task<(bool Ok, int? Id, IEnumerable<string> Errors)> CreateAsync(ProductCreateDto dto, CancellationToken ct = default)
    {
        var errors = _validator.ValidateProduct(dto).Select(e => e.Message).ToArray();
        if (errors.Length > 0) return (false, null, errors);

        var entity = new Product { Name = dto.Name.Trim(), Price = dto.Price, Quantity = dto.Quantity };
        var id = await _repo.CreateAsync(entity, ct);
        return (true, id, Array.Empty<string>());
    }

    public async Task<(bool Ok, IEnumerable<string> Errors)> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct = default)
    {
        var errors = _validator.ValidateProduct(dto).Select(e => e.Message).ToArray();
        if (errors.Length > 0) return (false, errors);

        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return (false, new[] { "Product not found." });

        existing.Name = dto.Name.Trim();
        existing.Price = dto.Price;
        existing.Quantity = dto.Quantity;
        existing.UpdatedAt = DateTime.UtcNow;

        var ok = await _repo.UpdateAsync(existing, ct);
        return (ok, ok ? Array.Empty<string>() : new[] { "Unable to update product." });
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

    private static ProductDto Map(Product e) => new(e.ProductId, e.Name, e.Price, e.Quantity);
}