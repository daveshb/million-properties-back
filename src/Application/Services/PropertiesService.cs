using MyApp.Domain.Entities;
using MyApp.Domain.Ports;

namespace MyApp.Application.Services;

public class PropertiesService : IPropertiesService
{
    private readonly IPropertiesRepository _repo;
    public PropertiesService(IPropertiesRepository repo) => _repo = repo;

    public Task<IEnumerable<Properties>> GetAllAsync(CancellationToken ct = default) =>
        _repo.GetAllAsync(ct);

    public Task<IEnumerable<Properties>> GetByNameAsync(string name, CancellationToken ct = default) =>
        _repo.GetByNameAsync(name, ct);

    public Task<IEnumerable<Properties>> GetByAddressAsync(string address, CancellationToken ct = default) =>
        _repo.GetByAddressAsync(address, ct);

    public Task<IEnumerable<Properties>> GetByPriceRangeAsync(double? minPrice, double? maxPrice, CancellationToken ct = default) =>
        _repo.GetByPriceRangeAsync(minPrice, maxPrice, ct);

    public Task<Properties?> GetByIdAsync(string id, CancellationToken ct = default) =>
        _repo.GetByIdAsync(id, ct);

    public async Task<Properties> CreateAsync(int idOwner, string name, double price, string address, string img, CancellationToken ct = default)
    {
        var properties = new Properties(idOwner, name, price, address, img);
        await _repo.AddAsync(properties, ct);
        return properties;
    }

    public async Task<bool> UpdateAsync(string id, string name, double price, string address, string img, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing == null) return false;
        existing.Update(name, price, address, img);
        await _repo.UpdateAsync(existing, ct);
        return true;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing == null) return false;
        await _repo.DeleteAsync(id, ct);
        return true;
    }
}