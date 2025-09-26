using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IPropertiesRepository
{
    Task<Properties?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetByNameAsync(string name, CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetByAddressAsync(string address, CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetByPriceRangeAsync(double? minPrice, double? maxPrice, CancellationToken ct = default);
    Task AddAsync(Properties properties, CancellationToken ct = default);
    Task UpdateAsync(Properties properties, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}