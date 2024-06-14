using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Repositories.Interfaces;

public interface INoSqlRepository<TEntity> : IRepository<TEntity> where TEntity : Entity;