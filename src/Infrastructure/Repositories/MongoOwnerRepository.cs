using MyApp.Domain.Ports;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Entities;
using MongoDB.Driver;

namespace MyApp.Infrastructure.Repositories;

public class MongoOwnerRepository : IOwnerRepository
{
    private readonly MongoDbContext _db;
    public MongoOwnerRepository(MongoDbContext db) => _db = db;

    public async Task<Owner?> GetByIdOwnerAsync(int idOwner, CancellationToken ct = default)
    {
        var e = await _db.Owners.Find(x => x.IdOwner == idOwner).FirstOrDefaultAsync(ct);
        if (e == null) return null;
        var owner = new Owner(e.IdOwner, e.Name, e.Email, e.Phone);
        typeof(Owner).GetProperty("Id")!.SetValue(owner, e.Id);
        return owner;
    }
}
