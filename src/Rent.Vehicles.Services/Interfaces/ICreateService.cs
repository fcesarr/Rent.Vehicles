using LanguageExt.Common;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface ICreateService<TEntity> where TEntity : Entity
{
    Task<Result<TEntity>> CreateAsync(TEntity? entity, CancellationToken cancellationToken = default);    
}