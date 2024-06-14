
using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class NoSqlService<TEntity> : Service<TEntity> where TEntity : Entity
{
    public NoSqlService(ILogger<Service<TEntity>> logger,
        IValidator<TEntity> validator,
        IRepository<TEntity> repository) : base(logger, validator, repository)
    {
    }
}

