using System.ComponentModel.DataAnnotations;

namespace Accounts.Contracts.Models;

public sealed record AccountModel
{
    [Required][MaxLength(100)] public required string UserId { get; set; }

    [Required][MaxLength(30)] public required string Name { get; set; }
}