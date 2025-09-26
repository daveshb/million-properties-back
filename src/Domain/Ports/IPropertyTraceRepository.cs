using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IPropertyTraceRepository
{
    Task<IEnumerable<PropertyTrace>> GetByIdPropertyAsync(int idProperty, CancellationToken ct = default);
}
