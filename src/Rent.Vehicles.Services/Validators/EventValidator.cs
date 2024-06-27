using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Validators;

public class EventValidator : Validator<Event>, IValidator<Event>;