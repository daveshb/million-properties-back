using MyApp.Domain.Ports;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Entities;
using MongoDB.Driver;

namespace MyApp.Infrastructure.Repositories;

public class MongoPropertyTraceRepository : IPropertyTraceRepository
{
    private readonly MongoDbContext _db;
    public MongoPropertyTraceRepository(MongoDbContext db) => _db = db;

    public async Task<IEnumerable<PropertyTrace>> GetByIdPropertyAsync(int idProperty, CancellationToken ct = default)
    {
        var entities = await _db.PropertyTraces.Find(x => x.IdProperty == idProperty).ToListAsync(ct);
        return entities.Select(e => 
        {
            var propertyTrace = new PropertyTrace(e.IdProperty, e.DateSale, e.Name, e.Value, e.Tax);
            typeof(PropertyTrace).GetProperty("Id")!.SetValue(propertyTrace, e.Id);
            return propertyTrace;
        });
    }
}
