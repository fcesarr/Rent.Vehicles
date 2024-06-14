using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Repositories.Interfaces;

public interface ISqlRepository<TEntity> : IRepository<TEntity> where TEntity : Entity;