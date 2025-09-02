using RealEstate.Domain.Entities;

namespace RealEstate.Domain.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync(string? q, CancellationToken ct = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(Product product, CancellationToken ct = default);
    Task<bool> UpdateAsync(Product product, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
