using RealEstate.Domain.Entities;

namespace RealEstate.Domain.Interfaces;

public interface IPropertyRepository
{
    Task<(IReadOnlyList<Property> Items, long Total)> SearchAsync(
        string? name, string? address, decimal? minPrice, decimal? maxPrice,
        int page, int pageSize, CancellationToken ct);

    Task<Property?> GetByIdAsync(string id, CancellationToken ct);
}
