using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;

namespace RealEstate.Infrastructure.EF.Repositories;

public class EfProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _ctx;
    public EfProductRepository(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Product>> GetAllAsync(string? q, CancellationToken ct = default)
    {
        var qry = _ctx.Products.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var t = q.Trim().ToLower();
            qry = qry.Where(p => p.Name.ToLower().Contains(t));
        }
        return await qry.OrderBy(p => p.ProductId).ToListAsync(ct);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        => _ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id, ct);

    public async Task<int> CreateAsync(Product product, CancellationToken ct = default)
    {
        _ctx.Products.Add(product);
        await _ctx.SaveChangesAsync(ct);
        return product.ProductId;
    }

    public async Task<bool> UpdateAsync(Product product, CancellationToken ct = default)
    {
        _ctx.Products.Update(product);
        var rows = await _ctx.SaveChangesAsync(ct);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var e = await _ctx.Products.FirstOrDefaultAsync(x => x.ProductId == id, ct);
        if (e is null) return false;
        _ctx.Products.Remove(e);
        return await _ctx.SaveChangesAsync(ct) > 0;
    }
}
