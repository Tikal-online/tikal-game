namespace Lobbies.Contracts.Models;

public sealed record LobbyModel
{
    public required long Id { get; set; }

    public required string Name { get; set; }

    public required int MaxPlayers { get; set; }

    public List<LobbyPlayerModel> Players { get; set; } = [];
}