
using System.Collections.Concurrent;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;

namespace RealEstate.Infrastructure.Memory.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<int, Product> _store = new();
    private int _seq = 100;

    public InMemoryProductRepository()
    {
        // seed
        CreateAsync(new Product{ Name="Cola Can 330ml", Price=2.00m, Quantity=500 }).Wait();
        CreateAsync(new Product{ Name="Cola 2L",        Price=5.00m, Quantity=200 }).Wait();
        CreateAsync(new Product{ Name="Orange 1L",      Price=4.50m, Quantity=150 }).Wait();
        CreateAsync(new Product{ Name="Water 600ml",    Price=1.50m, Quantity=800 }).Wait();
        CreateAsync(new Product{ Name="Tonic 1L",       Price=3.80m, Quantity=120 }).Wait();
    }

    public Task<IEnumerable<Product>> GetAllAsync(string? q, CancellationToken ct = default)
    {
        IEnumerable<Product> data = _store.Values.OrderBy(x => x.ProductId);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var t = q.Trim().ToLower();
            data = data.Where(p => p.Name.ToLower().Contains(t));
        }
        return Task.FromResult(data);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(id, out var p) ? p : null);

    public Task<int> CreateAsync(Product product, CancellationToken ct = default)
    {
        var id = Interlocked.Increment(ref _seq);
        product.ProductId = id;
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        _store[id] = product;
        return Task.FromResult(id);
    }

    public Task<bool> UpdateAsync(Product product, CancellationToken ct = default)
    {
        if (!_store.ContainsKey(product.ProductId)) return Task.FromResult(false);
        product.UpdatedAt = DateTime.UtcNow;
        _store[product.ProductId] = product;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_store.TryRemove(id, out _));
}
