using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.EF;
using RealEstate.Domain.Interfaces;

namespace RealEstate.Infrastructure.EF.Repositories;

public class EfPropertyRepository(ApplicationDbContext db) : IPropertyRepository
{
    public async Task<(IReadOnlyList<Property> Items, long Total)> SearchAsync(
        string? name, string? address, decimal? minPrice, decimal? maxPrice,
        int page, int pageSize, CancellationToken ct)
    {
        var q = db.Properties
            .AsNoTracking()
            .Include(p => p.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            q = q.Where(p => p.Name.Contains(name));
        if (!string.IsNullOrWhiteSpace(address))
            q = q.Where(p => p.Address.Contains(address));
        if (minPrice.HasValue)
            q = q.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue)
            q = q.Where(p => p.Price <= maxPrice.Value);

        var total = await q.LongCountAsync(ct);
        var items = await q.OrderBy(p => p.Price)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Property?> GetByIdAsync(string id, CancellationToken ct)
        => await db.Properties
            .AsNoTracking()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.IdProperty == id, ct);
}
