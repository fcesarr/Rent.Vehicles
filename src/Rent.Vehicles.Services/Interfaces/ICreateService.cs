using LanguageExt.Common;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface ICreateService<TEntity> : IAction<TEntity> where TEntity : class
{
    Task<Result<TEntity>> CreateAsync(TEntity? entity, CancellationToken cancellationToken = default);
}