namespace Games.Domain.EventData;

public sealed record GameEventData
{
    public required long Id { get; set; }

    public required long LobbyId { get; set; }

    public List<PlayerEventData> Players { get; set; } = [];

    public List<TileEventData> Tiles { get; set; } = [];
}