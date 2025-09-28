using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IPropertiesRepository
{
    Task<Properties?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetAllAsync(CancellationToken ct = default);
    Task<(IEnumerable<Properties> Items, int TotalCount)> GetAllPaginatedAsync(int pageNumber, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetByNameAsync(string name, CancellationToken ct = default);
    Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNamePaginatedAsync(string name, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetByAddressAsync(string address, CancellationToken ct = default);
    Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAddressPaginatedAsync(string address, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<Properties>> GetByPriceRangeAsync(double? minPrice, double? maxPrice, CancellationToken ct = default);
    Task<(IEnumerable<Properties> Items, int TotalCount)> GetByPriceRangePaginatedAsync(double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNameAndAddressPaginatedAsync(string name, string address, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<(IEnumerable<Properties> Items, int TotalCount)> GetByNameAndPriceRangePaginatedAsync(string name, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAddressAndPriceRangePaginatedAsync(string address, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<(IEnumerable<Properties> Items, int TotalCount)> GetByAllFiltersPaginatedAsync(string name, string address, double? minPrice, double? maxPrice, int pageNumber, int pageSize, CancellationToken ct = default);
    Task AddAsync(Properties properties, CancellationToken ct = default);
    Task UpdateAsync(Properties properties, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}