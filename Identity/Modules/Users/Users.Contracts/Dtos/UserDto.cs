using System.ComponentModel.DataAnnotations;

namespace Users.Contracts.Dtos;

public record UserDto
{
    [Required]
    [MinLength(6)]
    [MaxLength(30)]
    public required string Username { get; init; }
}