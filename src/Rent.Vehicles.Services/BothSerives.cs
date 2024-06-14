using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class BothServices<TEntity> : IBothServices<TEntity> where TEntity : Entity
{
    private readonly ILogger<BothServices<TEntity>> _logger; 

    private readonly IValidator<TEntity> _validator;

    private readonly ISqlRepository<TEntity> _sqlRepository;

    private readonly INoSqlRepository<TEntity> _noSqlRepository;

    public BothServices(ILogger<BothServices<TEntity>> logger,
        IValidator<TEntity> validator,
        ISqlRepository<TEntity> sqlRepository,
        INoSqlRepository<TEntity> noSqlRepository)
    {
        _logger = logger;
        _validator = validator;
        _sqlRepository = sqlRepository;
        _noSqlRepository = noSqlRepository;
    }

    public async Task CreateAsync(TEntity? entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, cancellationToken);

        if(!result.IsValid)
            throw result.Exception ?? new Exception();

        await _sqlRepository.CreateAsync(entity!, cancellationToken);
        await _noSqlRepository.CreateAsync(entity!, cancellationToken);
    }
}
