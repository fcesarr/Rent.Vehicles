using System.ComponentModel.DataAnnotations;

using Rent.Vehicles.Lib.Attributes;


namespace Rent.Vehicles.Messages.Commands;

[MessagePack.MessagePackObject]
public record UpdateRentCommand : Command
{
    [MessagePack.Key(1)]
    [Required]
    public Guid Id { get; init; }

    [MessagePack.Key(2)]
    [Required]
    [DateTimeMinorCurrentDate(ErrorMessage ="Invalid date")]
    public required DateTime EstimatedDate { get; set; }
}
