namespace Lobbies.Contracts.Models;

public sealed record ChatMessageModel
{
    public required string UserId { get; set; }

    public required string Username { get; set; }

    public required string Content { get; set; }
}