
using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public abstract class Service<TEntity> : IService<TEntity> where TEntity : Entity
{
    protected readonly ILogger<Service<TEntity>> _logger; 

    private readonly IValidator<TEntity> _validator;

    protected readonly IRepository<TEntity> _repository;

    public Service(ILogger<Service<TEntity>> logger,
        IValidator<TEntity> validator,
        IRepository<TEntity> repository)
    {
        _logger = logger;
        _validator = validator;
        _repository = repository;
    }

    public async Task CreateAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, cancellationToken);

        if(!result.IsValid)
            throw result.Exception ?? new Exception();

        await _repository.CreateAsync(result.Instance, cancellationToken);
    }

    public async Task DeleteAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, cancellationToken);

        if(!result.IsValid)
            throw result.Exception ?? new Exception();

        await _repository.DeleteAsync(result.Instance, cancellationToken);
    }

    public Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _repository.FindAsync(predicate, cancellationToken);
    }

    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _repository.GetAsync(predicate, cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, cancellationToken);

        if(!result.IsValid)
            throw result.Exception ?? new Exception();

        await _repository.UpdateAsync(result.Instance, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(x => x.Id == id, cancellationToken);

        await DeleteAsync(entity, cancellationToken);
    }
}

