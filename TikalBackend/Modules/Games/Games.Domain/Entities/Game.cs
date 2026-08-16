namespace Games.Domain.Entities;

public sealed class Game
{
    public long Id { get; set; }

    public required long LobbyId { get; set; }

    public ICollection<Player> Players { get; set; } = [];

    public TileMap TileMap { get; set; } = null!;
}