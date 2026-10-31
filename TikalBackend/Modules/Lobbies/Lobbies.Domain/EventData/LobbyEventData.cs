namespace Lobbies.Domain.EventData;

public sealed record LobbyEventData
{
    public required long Id { get; set; }

    public required string Name { get; set; }

    public required int MaxPlayers { get; set; }

    public List<PlayerEventData> Players { get; set; } = [];

    public bool InGame { get; set; }
}