namespace Accounts.Contracts.Models;

public sealed record AccountModel
{
    public required string UserId { get; set; }

    public required string Name { get; set; }
}