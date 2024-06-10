using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IService<in T> : ICreateService<T> where T : Entity
{
}