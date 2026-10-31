namespace Shared.Api;

public record UserAccount
{
    public required string UserId { get; set; }
    public required string Name { get; set; }
}

public sealed record AccountContext
{
    public UserAccount Account { get; set; } = null!;
}