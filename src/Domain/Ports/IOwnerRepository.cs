using MyApp.Domain.Entities;

namespace MyApp.Domain.Ports;

public interface IOwnerRepository
{
    Task<Owner?> GetByIdOwnerAsync(int idOwner, CancellationToken ct = default);
}
