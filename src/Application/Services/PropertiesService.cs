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

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetAllPaginatedAsync(int pageNumber, int pageSize, CancellationToken ct = default) =>
        _repo.GetAllPaginatedAsync(pageNumber, pageSize, ct);

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNamePaginatedAsync(string name, int pageNumber, int pageSize, CancellationToken ct = default) =>
        _repo.GetByNamePaginatedAsync(name, pageNumber, pageSize, ct);

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAddressPaginatedAsync(string address, int pageNumber, int pageSize, CancellationToken ct = default) =>
        _repo.GetByAddressPaginatedAsync(address, pageNumber, pageSize, ct);

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByPriceRangePaginatedAsync(double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default) =>
        _repo.GetByPriceRangePaginatedAsync(minPrice, maxPrice, pageNumber, pageSize, ct);

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNameAndAddressPaginatedAsync(string name, string address, int pageNumber, int pageSize, CancellationToken ct = default) =>
        _repo.GetByNameAndAddressPaginatedAsync(name, address, pageNumber, pageSize, ct);

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNameAndPriceRangePaginatedAsync(string name, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default) =>
        _repo.GetByNameAndPriceRangePaginatedAsync(name, minPrice, maxPrice, pageNumber, pageSize, ct);

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAddressAndPriceRangePaginatedAsync(string address, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default) =>
        _repo.GetByAddressAndPriceRangePaginatedAsync(address, minPrice, maxPrice, pageNumber, pageSize, ct);

    public Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAllFiltersPaginatedAsync(string name, string address, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default) =>
        _repo.GetByAllFiltersPaginatedAsync(name, address, minPrice, maxPrice, pageNumber, pageSize, ct);

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