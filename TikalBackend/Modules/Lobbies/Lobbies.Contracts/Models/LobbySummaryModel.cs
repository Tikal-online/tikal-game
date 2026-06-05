namespace Lobbies.Contracts.Models;

public sealed record LobbySummaryModel
{
    public required long Id { get; set; }

    public required string Name { get; set; }

    public required int MaxPlayers { get; set; }

    public required int CurrentPlayers { get; set; }
}