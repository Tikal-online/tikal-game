namespace Games.Contracts.Models;

public sealed record GameModel
{
    public required long Id { get; set; }

    public List<GamePlayerModel> Players { get; set; } = [];

    public List<TileModel> Tiles { get; init; } = [];
}