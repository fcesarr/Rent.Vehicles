using LanguageExt.Common;

using Rent.Vehicles.Entities;

namespace Rent.Vehicles.Services.Interfaces;

public interface IFacade<TObject, TEntity> 
    where TObject : class
    where TEntity : class
{
    Task<Result<TEntity>> CreateAsync(TObject? entity, CancellationToken cancellationToken = default);
}