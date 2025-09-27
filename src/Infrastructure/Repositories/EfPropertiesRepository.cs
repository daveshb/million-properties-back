
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

    public async Task<(IEnumerable<Properties> Items, int TotalCount)> GetAllPaginatedAsync(int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var skip = (pageNumber - 1) * pageSize;
        var totalCount = await _db.Properties.CountDocumentsAsync(_ => true, cancellationToken: ct);
        var entities = await _db.Properties.Find(_ => true).Skip(skip).Limit(pageSize).ToListAsync(ct);
        
        var items = entities.Select(e => 
        {
            var prop = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img, e.IdProperty, e.CodeInternal, e.Year);
            typeof(Properties).GetProperty("Id")!.SetValue(prop, e.Id);
            return prop;
        });
        
        return (items, (int)totalCount);
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

    public async Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNamePaginatedAsync(string name, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filter = Builders<PropertiesEntity>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));
        var skip = (pageNumber - 1) * pageSize;
        var totalCount = await _db.Properties.CountDocumentsAsync(filter, cancellationToken: ct);
        var entities = await _db.Properties.Find(filter).Skip(skip).Limit(pageSize).ToListAsync(ct);
        
        var items = entities.Select(e => 
        {
            var prop = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img, e.IdProperty, e.CodeInternal, e.Year);
            typeof(Properties).GetProperty("Id")!.SetValue(prop, e.Id);
            return prop;
        });
        
        return (items, (int)totalCount);
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

    // Helper method for pagination
    private async Task<(IEnumerable<Properties> Items, int TotalCount)> GetPaginatedAsync(
        MongoDB.Driver.FilterDefinition<PropertiesEntity> filter, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var skip = (pageNumber - 1) * pageSize;
        var totalCount = await _db.Properties.CountDocumentsAsync(filter, cancellationToken: ct);
        var entities = await _db.Properties.Find(filter).Skip(skip).Limit(pageSize).ToListAsync(ct);
        
        var items = entities.Select(e => 
        {
            var prop = new Properties(e.IdOwner, e.Name, e.Price, e.Address, e.Img, e.IdProperty, e.CodeInternal, e.Year);
            typeof(Properties).GetProperty("Id")!.SetValue(prop, e.Id);
            return prop;
        });
        
        return (items, (int)totalCount);
    }

    // Pagination methods for different filter combinations
    public async Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAddressPaginatedAsync(string address, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filter = Builders<PropertiesEntity>.Filter.Regex(x => x.Address, new MongoDB.Bson.BsonRegularExpression(address, "i"));
        return await GetPaginatedAsync(filter, pageNumber, pageSize, ct);
    }

    public async Task<(IEnumerable<Properties> Items, int TotalCount)> GetByPriceRangePaginatedAsync(double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filterBuilder = Builders<PropertiesEntity>.Filter;
        var filters = new List<MongoDB.Driver.FilterDefinition<PropertiesEntity>>();

        if (minPrice.HasValue)
            filters.Add(filterBuilder.Gte(x => x.Price, minPrice.Value));

        if (maxPrice.HasValue)
            filters.Add(filterBuilder.Lte(x => x.Price, maxPrice.Value));

        var filter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;
        return await GetPaginatedAsync(filter, pageNumber, pageSize, ct);
    }

    public async Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNameAndAddressPaginatedAsync(string name, string address, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var nameFilter = Builders<PropertiesEntity>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));
        var addressFilter = Builders<PropertiesEntity>.Filter.Regex(x => x.Address, new MongoDB.Bson.BsonRegularExpression(address, "i"));
        var filter = Builders<PropertiesEntity>.Filter.And(nameFilter, addressFilter);
        return await GetPaginatedAsync(filter, pageNumber, pageSize, ct);
    }

    public async Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNameAndPriceRangePaginatedAsync(string name, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var nameFilter = Builders<PropertiesEntity>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));
        var priceFilters = new List<MongoDB.Driver.FilterDefinition<PropertiesEntity>> { nameFilter };

        if (minPrice.HasValue)
            priceFilters.Add(Builders<PropertiesEntity>.Filter.Gte(x => x.Price, minPrice.Value));

        if (maxPrice.HasValue)
            priceFilters.Add(Builders<PropertiesEntity>.Filter.Lte(x => x.Price, maxPrice.Value));

        var filter = Builders<PropertiesEntity>.Filter.And(priceFilters);
        return await GetPaginatedAsync(filter, pageNumber, pageSize, ct);
    }

    public async Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAddressAndPriceRangePaginatedAsync(string address, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var addressFilter = Builders<PropertiesEntity>.Filter.Regex(x => x.Address, new MongoDB.Bson.BsonRegularExpression(address, "i"));
        var priceFilters = new List<MongoDB.Driver.FilterDefinition<PropertiesEntity>> { addressFilter };

        if (minPrice.HasValue)
            priceFilters.Add(Builders<PropertiesEntity>.Filter.Gte(x => x.Price, minPrice.Value));

        if (maxPrice.HasValue)
            priceFilters.Add(Builders<PropertiesEntity>.Filter.Lte(x => x.Price, maxPrice.Value));

        var filter = Builders<PropertiesEntity>.Filter.And(priceFilters);
        return await GetPaginatedAsync(filter, pageNumber, pageSize, ct);
    }

    public async Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAllFiltersPaginatedAsync(string name, string address, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var filters = new List<MongoDB.Driver.FilterDefinition<PropertiesEntity>>();

        if (!string.IsNullOrEmpty(name))
            filters.Add(Builders<PropertiesEntity>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(name, "i")));

        if (!string.IsNullOrEmpty(address))
            filters.Add(Builders<PropertiesEntity>.Filter.Regex(x => x.Address, new MongoDB.Bson.BsonRegularExpression(address, "i")));

        if (minPrice.HasValue)
            filters.Add(Builders<PropertiesEntity>.Filter.Gte(x => x.Price, minPrice.Value));

        if (maxPrice.HasValue)
            filters.Add(Builders<PropertiesEntity>.Filter.Lte(x => x.Price, maxPrice.Value));

        var filter = filters.Count > 0 ? Builders<PropertiesEntity>.Filter.And(filters) : Builders<PropertiesEntity>.Filter.Empty;
        return await GetPaginatedAsync(filter, pageNumber, pageSize, ct);
    }
}