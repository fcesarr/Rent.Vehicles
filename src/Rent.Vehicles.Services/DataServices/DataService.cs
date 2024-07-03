using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public abstract class DataService<TEntity> : IDataService<TEntity> where TEntity : Entity
{
    protected readonly ILogger<DataService<TEntity>> _logger;

    protected readonly IRepository<TEntity> _repository;

    protected readonly IValidator<TEntity> _validator;

    public DataService(ILogger<DataService<TEntity>> logger,
        IValidator<TEntity> validator,
        IRepository<TEntity> repository)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
    }

    public virtual async Task<Result<TEntity>> CreateAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception!;
        }

        return await _repository.CreateAsync(result.Instance, cancellationToken);
    }

    public virtual async Task<Result<bool>> DeleteAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception!;
        }

        await _repository.DeleteAsync(result.Instance, cancellationToken);

        return true;
    }

    public virtual async Task<Result<IEnumerable<TEntity>>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        CancellationToken cancellationToken = default)
    {
        var entities =
            await _repository.FindAsync(predicate, descending, orderBy, cancellationToken: cancellationToken);

        if (!entities.Any())
        {
            return new EmptyException($"Entities {typeof(TEntity).Name} is empty");
        }

        return entities
            .ToList();
    }

    public virtual async Task<Result<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsync(predicate, cancellationToken: cancellationToken);

        if (entity == null)
        {
            return new NullException($"Entity {typeof(TEntity).Name} not found");
        }

        return entity;
    }

    public virtual async Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, cancellationToken);

        if (!result.IsValid)
        {
            return result.Exception!;
        }

        result.Instance.Updated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);

        return await _repository.UpdateAsync(result.Instance, cancellationToken);
    }

    public virtual async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return await DeleteAsync(entity.Value, cancellationToken);
    }
}
