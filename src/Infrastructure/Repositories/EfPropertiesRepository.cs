
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

    private static Properties MapToDomain(PropertiesEntity e) => new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img) { };

    private static PropertiesEntity MapToEntity(Properties u) =>
        new PropertiesEntity { Id = u.Id, Name = u.Name, IdOwner = u.IdOwner, Price = u.Price, Address = u.Address, Img = u.Img };

    public async Task AddAsync(Properties properties, CancellationToken ct = default)
    {
        var entity = new PropertiesEntity { Name = properties.Name, IdOwner = properties.IdOwner, Price = properties.Price, Address = properties.Address, Img = properties.Img };
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
            var prop = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img);
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
            var prop = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img);
            typeof(Properties).GetProperty("Id")!.SetValue(prop, e.Id);
            return prop;
        });
    }

    public async Task<Properties?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var e = await _db.Properties.Find(x => x.Id == id).FirstOrDefaultAsync(ct);
        if (e == null) return null;
        var u = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img);
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
            .Set(x => x.Img, properties.Img);
        
        var result = await _db.Properties.UpdateOneAsync(filter, update, cancellationToken: ct);
        if (result.MatchedCount == 0)
            throw new InvalidOperationException("Properties not found");
    }
}