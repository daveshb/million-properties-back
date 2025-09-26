
using MyApp.Domain.Ports;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Entities;
using MongoDB.Driver;

namespace MyApp.Infrastructure.Repositories;

public class MongoPropertiesRepository : IPropertiesRepository
{
    private readonly MongoDbContext _db;
    public MongoPropertiesRepository(MongoDbContext db) => _db = db;

    private static Properties MapToDomain(PropertiesEntity e) => new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img, e.IdProperty, e.CodeInternal, e.Year) { };

    private static PropertiesEntity MapToEntity(Properties u) =>
        new PropertiesEntity { Id = u.Id, Name = u.Name, IdOwner = u.IdOwner, Price = u.Price, Address = u.Address, Img = u.Img, IdProperty = u.IdProperty, CodeInternal = u.CodeInternal, Year = u.Year };

    public async Task AddAsync(Properties properties, CancellationToken ct = default)
    {
        var entity = new PropertiesEntity { Name = properties.Name, IdOwner = properties.IdOwner, Price = properties.Price, Address = properties.Address, Img = properties.Img, IdProperty = properties.IdProperty, CodeInternal = properties.CodeInternal, Year = properties.Year };
        await _db.Properties.InsertOneAsync(entity, cancellationToken: ct);
        // actualizar Id en el dominio
        typeof(Properties).GetProperty("Id")!.SetValue(properties, entity.Id);
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        await _db.Properties.DeleteOneAsync(x => x.Id == id, ct);
    }

    public async Task<IEnumerable<Properties>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _db.Properties.Find(_ => true).ToListAsync(ct);
        return entities.Select(e => 
        {
            var prop = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img, e.IdProperty, e.CodeInternal, e.Year);
            typeof(Properties).GetProperty("Id")!.SetValue(prop, e.Id);
            return prop;
        });
    }

    public async Task<IEnumerable<Properties>> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var filter = Builders<PropertiesEntity>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));
        var entities = await _db.Properties.Find(filter).ToListAsync(ct);
        return entities.Select(e => 
        {
            var prop = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img, e.IdProperty, e.CodeInternal, e.Year);
            typeof(Properties).GetProperty("Id")!.SetValue(prop, e.Id);
            return prop;
        });
    }

    public async Task<IEnumerable<Properties>> GetByAddressAsync(string address, CancellationToken ct = default)
    {
        var filter = Builders<PropertiesEntity>.Filter.Regex(x => x.Address, new MongoDB.Bson.BsonRegularExpression(address, "i"));
        var entities = await _db.Properties.Find(filter).ToListAsync(ct);
        return entities.Select(e => 
        {
            var prop = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img, e.IdProperty, e.CodeInternal, e.Year);
            typeof(Properties).GetProperty("Id")!.SetValue(prop, e.Id);
            return prop;
        });
    }

    public async Task<IEnumerable<Properties>> GetByPriceRangeAsync(double? minPrice, double? maxPrice, CancellationToken ct = default)
    {
        var filterBuilder = Builders<PropertiesEntity>.Filter;
        var filters = new List<MongoDB.Driver.FilterDefinition<PropertiesEntity>>();

        if (minPrice.HasValue)
            filters.Add(filterBuilder.Gte(x => x.Price, minPrice.Value));

        if (maxPrice.HasValue)
            filters.Add(filterBuilder.Lte(x => x.Price, maxPrice.Value));

        var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;
        var entities = await _db.Properties.Find(filter).ToListAsync(ct);
        return entities.Select(e => 
        {
            var prop = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img, e.IdProperty, e.CodeInternal, e.Year);
            typeof(Properties).GetProperty("Id")!.SetValue(prop, e.Id);
            return prop;
        });
    }

    public async Task<Properties?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var e = await _db.Properties.Find(x => x.Id == id).FirstOrDefaultAsync(ct);
        if (e == null) return null;
        var u = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img, e.IdProperty, e.CodeInternal, e.Year);
        typeof(Properties).GetProperty("Id")!.SetValue(u, e.Id);
        return u;
    }

    public async Task UpdateAsync(Properties properties, CancellationToken ct = default)
    {
        var filter = Builders<PropertiesEntity>.Filter.Eq(x => x.Id, properties.Id);
        var update = Builders<PropertiesEntity>.Update
            .Set(x => x.Name, properties.Name)
            .Set(x => x.Price, properties.Price)
            .Set(x => x.Address, properties.Address)
            .Set(x => x.Img, properties.Img)
            .Set(x => x.IdProperty, properties.IdProperty)
            .Set(x => x.CodeInternal, properties.CodeInternal)
            .Set(x => x.Year, properties.Year);
        
        var result = await _db.Properties.UpdateOneAsync(filter, update, cancellationToken: ct);
        if (result.MatchedCount == 0)
            throw new InvalidOperationException("Properties not found");
    }
}