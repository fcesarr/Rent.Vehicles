using Microsoft.Extensions.Logging;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services;

public class UserDataService : DataService<User>, IUserDataService
{
    public UserDataService(ILogger<UserDataService> logger,
        IUserValidator validator,
        IRepository<User> repository) : base(logger, validator, repository)
    {
    }
}