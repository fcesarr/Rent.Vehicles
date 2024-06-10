using Rent.Vehicles.Messages;

namespace Rent.Vehicles.Services.Interfaces;

public interface ICreateService<T> where T : Message
{
    Task Create(T message, CancellationToken cancellationToken = default);
}