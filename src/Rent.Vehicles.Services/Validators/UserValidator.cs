using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Validators;

public class UserValidator : Validator<User>, IUserValidator;
