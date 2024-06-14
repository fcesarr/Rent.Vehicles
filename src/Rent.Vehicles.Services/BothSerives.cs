using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class BothSerives<TEntity> : Service<TEntity> where TEntity : Entity
{
    public BothSerives(ILogger<BothSerives<TEntity>> logger,
        IValidator<TEntity> validator,
        ISqlRepository<TEntity> sqlRepository,
        INoSqlRepository<TEntity> noSqlRepository) : base(logger, validator, sqlRepository)
    {
    }
}
