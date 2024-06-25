
using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Exceptions;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class DataService<TEntity> : IDataService<TEntity> where TEntity : Entity
{
    protected readonly ILogger<DataService<TEntity>> _logger; 

    private readonly IValidator<TEntity> _validator;

    protected readonly IRepository<TEntity> _repository;

    public DataService(ILogger<DataService<TEntity>> logger,
        IValidator<TEntity> validator,
        IRepository<TEntity> repository)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
    }

    public async Task<Result<TEntity>> CreateAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, cancellationToken);

        if(!result.IsValid)
            return Result<TEntity>.Failure(result.Exception);

        await _repository.CreateAsync(result.Instance, cancellationToken);

        return result.Instance;
    }

    public async Task<Result<bool>> DeleteAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, cancellationToken);

        if(!result.IsValid)
            return Result<bool>.Failure(result.Exception);

        await _repository.DeleteAsync(result.Instance, cancellationToken);

        return true;
    }

    public async Task<Result<IEnumerable<TEntity>>> FindAsync(Expression<Func<TEntity, bool>> predicate,
        bool descending = false,
        Expression<Func<TEntity, dynamic>>? orderBy = default,
        CancellationToken cancellationToken = default)
    {
        var entities = await _repository.FindAsync(predicate, descending, orderBy, cancellationToken: cancellationToken);
    
        if(!entities.Any())
            return Result<IEnumerable<TEntity>>.Failure(new EmptyException());

        return entities
            .ToList();
    }

    public async Task<Result<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsync(predicate, cancellationToken: cancellationToken);

        if(entity == null)
            return Result<TEntity>.Failure(new NullException());

        return entity;
    }

    public async Task<Result<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, cancellationToken);

        if(!result.IsValid)
            return Result<TEntity>.Failure(result.Exception);

        await _repository.UpdateAsync(result.Instance, cancellationToken);

        return entity;
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        if(!entity.IsSuccess)
            return Result<bool>.Failure(entity.Exception);

        return await DeleteAsync(entity.Value, cancellationToken);
    }
}

