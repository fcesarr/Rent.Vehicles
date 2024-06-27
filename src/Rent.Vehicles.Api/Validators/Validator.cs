



using System.ComponentModel.DataAnnotations;

using Rent.Vehicles.Messages;
using Rent.Vehicles.Services.Validators.Interfaces;

namespace Rent.Vehicles.Api.Validators;

public class Validator<TCommand> : IValidator<TCommand> where TCommand : Command
{
    public Task<ValidationResult<TCommand>> ValidateAsync(TCommand? command, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => {
            //
            var validationResult = new ValidationResult<TCommand>
            {
                IsValid = false,
            };

            if(command is null)
            {
                validationResult.Exception = new Services.Exceptions.ValidationException($"Error on Validate {typeof(TCommand).Name}",
                    new Dictionary<string, string[]>());
                return validationResult;
            }

            var context = new ValidationContext(command);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(command, context, results, true);

            validationResult.IsValid = isValid;

            if(!isValid)
            {
                var errors = results.SelectMany(result => result.MemberNames.Select(memberName => new Tuple<string, string>(memberName, result.ErrorMessage ?? string.Empty) ))
                    .GroupBy(x => x.Item1)
                    .ToDictionary<IGrouping<string, Tuple<string, string>>,string, string[]>(
                        g => g.Key, 
                        g => g.Select(x => x.Item2).ToArray()
                    );

                validationResult.Exception = new Services.Exceptions.ValidationException($"Error on Validate {typeof(TCommand).Name}",
                        errors);
            }

            return validationResult;
            //
        }, cancellationToken);
    }
}