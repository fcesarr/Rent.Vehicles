using Rent.Vehicles.Messages;

namespace Rent.Vehicles.Services.Interfaces;

public interface IService<T> : ICreateService<T> where T : Message
{
}