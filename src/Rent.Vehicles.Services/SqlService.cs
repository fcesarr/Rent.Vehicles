


using FluentValidation;

using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

namespace Rent.Vehicles.Services;

public sealed class SqlService<TEntity> : Service<TEntity> where TEntity : Entity
{
    public SqlService(ILogger<Service<TEntity>> logger,
        Validators.Interfaces.IValidator<TEntity> validator,
        IRepository<TEntity> repository) : base(logger, validator, repository)
    {
    }
}

