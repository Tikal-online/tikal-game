using System.ComponentModel.DataAnnotations;

namespace RestApi.Controllers.Accounts.Dtos;

public sealed record AccountDto
{
    [Required]
    [MaxLength(100)]
    public required string UserId { get; set; }

    [Required]
    [MaxLength(30)]
    public required string Name { get; set; }
}