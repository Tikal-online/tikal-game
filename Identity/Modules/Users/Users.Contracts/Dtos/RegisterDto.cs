using System.ComponentModel.DataAnnotations;

namespace Users.Contracts.Dtos;

public record RegisterDto
{
    [Required]
    [MinLength(6)]
    [MaxLength(30)]
    public required string Username { get; set; }

    [Required]
    [MinLength(8)]
    [MaxLength(100)]
    public required string Password { get; set; }
}