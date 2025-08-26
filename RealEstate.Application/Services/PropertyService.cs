using RealEstate.Application.Common;
using RealEstate.Application.DTOs;
using RealEstate.Application.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;    


namespace RealEstate.Application.Services;

public class PropertyService(IPropertyRepository repo) : IPropertyService
{
    public async Task<PagedResult<PropertyDto>> GetAsync(
        string? name, string? address, decimal? minPrice, decimal? maxPrice,
        int page, int pageSize, CancellationToken ct)
    {
        var (items, total) = await repo.SearchAsync(name, address, minPrice, maxPrice, page, pageSize, ct);
        return new PagedResult<PropertyDto>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Items = items.Select(Map).ToList()
        };
    }

    public async Task<PropertyDto?> GetByIdAsync(string id, CancellationToken ct)
        => (await repo.GetByIdAsync(id, ct)) is { } p ? Map(p) : null;

    private static PropertyDto Map(Property p)
    {
        var one = p.Images.FirstOrDefault(i => i.Enabled) ?? p.Images.FirstOrDefault();
        return new PropertyDto(
            p.IdProperty,
            p.IdOwner,
            p.Name,
            p.Address,
            p.Price,
            one?.File ?? string.Empty
        );
    }
}
