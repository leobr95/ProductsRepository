using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;
using RealEstate.Infrastructure.Mongo;

namespace RealEstate.Infrastructure.Mongo.Repositories;

// Document models (para no acoplar el dominio a Mongo)
[BsonIgnoreExtraElements]
public class MProperty
{
    // _id nativo de Mongo (opcional)
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string? MongoId { get; set; }

    // Tu id lógico (string)
    [BsonElement("IdProperty")]
    public string IdProperty { get; set; } = default!;

    [BsonElement("Name")]
    public string Name { get; set; } = default!;

    [BsonElement("Address")]
    public string Address { get; set; } = default!;

    // Evita pérdida de precisión si usas decimales
    [BsonRepresentation(BsonType.Decimal128)]
    [BsonElement("Price")]
    public decimal Price { get; set; }

    [BsonElement("CodeInternal")]
    public string? CodeInternal { get; set; }

    [BsonElement("Year")]
    public int? Year { get; set; }

    [BsonElement("IdOwner")]
    public string IdOwner { get; set; } = default!;

    [BsonElement("Images")]
    public List<MPropertyImage> Images { get; set; } = new();
}

[BsonIgnoreExtraElements]
public class MPropertyImage
{
    [BsonElement("IdPropertyImage")]
    public string? IdPropertyImage { get; set; }

    [BsonElement("IdProperty")]
    public string? IdProperty { get; set; }

    [BsonElement("File")]
    public string File { get; set; } = default!;

    [BsonElement("Enabled")]
    public bool Enabled { get; set; }
}
public class MongoPropertyRepository : IPropertyRepository
{
    private readonly IMongoCollection<MProperty> _col;

    public MongoPropertyRepository(IMongoDatabase db)
        => _col = db.GetCollection<MProperty>("properties");

    public async Task<(IReadOnlyList<Property> Items, long Total)> SearchAsync(
        string? name, string? address, decimal? minPrice, decimal? maxPrice,
        int page, int pageSize, CancellationToken ct)
    {
        var filter = Builders<MProperty>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(name))
            filter &= Builders<MProperty>.Filter.Regex(x => x.Name, new BsonRegularExpression(name, "i"));

        if (!string.IsNullOrWhiteSpace(address))
            filter &= Builders<MProperty>.Filter.Regex(x => x.Address, new BsonRegularExpression(address, "i"));

        if (minPrice.HasValue) filter &= Builders<MProperty>.Filter.Gte(x => x.Price, minPrice.Value);
        if (maxPrice.HasValue) filter &= Builders<MProperty>.Filter.Lte(x => x.Price, maxPrice.Value);

        var total = await _col.CountDocumentsAsync(filter, cancellationToken: ct);

        var docs = await _col.Find(filter)
            .SortBy(x => x.Price)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return (docs.Select(Map).ToList(), total);
    }

    public async Task<Property?> GetByIdAsync(string id, CancellationToken ct)
    {
        var doc = await _col.Find(x => x.IdProperty == id).FirstOrDefaultAsync(ct);
        return doc is null ? null : Map(doc);
    }

    private static Property Map(MProperty m) => new()
    {
        IdProperty = m.IdProperty,
        Name = m.Name,
        Address = m.Address,
        Price = m.Price,
        CodeInternal = m.CodeInternal,
        Year = m.Year,
        IdOwner = m.IdOwner,
        Images = m.Images.Select(i => new PropertyImage {
            IdPropertyImage = i.IdPropertyImage, IdProperty = i.IdProperty,
            File = i.File, Enabled = i.Enabled
        }).ToList()
    };
}
