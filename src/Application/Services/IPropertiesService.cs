using MyApp.Domain.Entities;

namespace MyApp.Application.Services;

public interface IPropertiesService
{
    Task<IEnumerable<Properties>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetByNameAsync(string name, CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetByAddressAsync(string address, CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetByPriceRangeAsync(double? minPrice, double? maxPrice, CancellationToken ct = default);
    Task<Properties?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Properties> CreateAsync(int idOwner, string name, double price, string address, string img, CancellationToken ct = default);
    Task<bool> UpdateAsync(string id, string name, double price, string address, string img, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}