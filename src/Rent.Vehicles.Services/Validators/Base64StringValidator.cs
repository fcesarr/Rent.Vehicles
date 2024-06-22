using FluentValidation;

using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Services.Validators;

public class Base64StringValidator : Validator<string>, IBase64StringValidator
{
    
}