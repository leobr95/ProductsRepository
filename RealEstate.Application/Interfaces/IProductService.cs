using RealEstate.Application.DTOs;

namespace RealEstate.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync(string? q, CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<(bool Ok, int? Id, IEnumerable<string> Errors)> CreateAsync(ProductCreateDto dto, CancellationToken ct = default);
    Task<(bool Ok, IEnumerable<string> Errors)> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
