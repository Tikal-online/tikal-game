using System.ComponentModel.DataAnnotations;

namespace RestApi.Controllers.Accounts.Dtos;

public sealed record CreateAccountDto
{
    [Required] [MaxLength(30)] public required string Name { get; set; }
}