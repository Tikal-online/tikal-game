using System.ComponentModel.DataAnnotations;
using Accounts.Contracts.Models;

namespace RestApi.Controllers.Accounts.Dtos;

public sealed record AccountDto
{
    [Required]
    [MaxLength(100)]
    public required string UserId { get; set; }

    [Required]
    [MaxLength(30)]
    public required string Name { get; set; }

    public static AccountDto FromModel(AccountModel model)
    {
        return new AccountDto { UserId = model.UserId, Name = model.Name };
    }
}