namespace Accounts.Domain.Entities;

public sealed class Account
{
    public long Id { get; set; }

    public required string UserId { get; set; }

    public required string Name { get; set; }
}