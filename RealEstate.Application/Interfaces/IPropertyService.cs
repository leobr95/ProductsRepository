using RealEstate.Application.Common;
using RealEstate.Application.DTOs;

namespace RealEstate.Application.Interfaces;

public interface IPropertyService
{
    Task<PagedResult<PropertyDto>> GetAsync(
        string? name, string? address, decimal? minPrice, decimal? maxPrice,
        int page, int pageSize, CancellationToken ct);

    Task<PropertyDto?> GetByIdAsync(string id, CancellationToken ct);
}
