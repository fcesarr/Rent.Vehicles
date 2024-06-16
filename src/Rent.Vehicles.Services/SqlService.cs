
using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class SqlService<TEntity> : Service<TEntity>, ISqlService<TEntity> where TEntity : Entity
{
    public SqlService(ILogger<Service<TEntity>> logger,
        IValidator<TEntity> validator,
        ISqlRepository<TEntity> repository) : base(logger, validator, repository)
    {
    }
}

