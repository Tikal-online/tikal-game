using Games.Domain.Events;
using Shared.Domain.Entities;

namespace Games.Domain.Entities;

public sealed class Game : Entity
{
    public long Id { get; set; }

    public required long LobbyId { get; set; }

    public ICollection<Player> Players { get; set; } = [];

    public List<Tile> Tiles { get; init; } = [];

    public bool HasEnoughPlayers => Players.Count > 1;

    public void RemovePlayer(Player player)
    {
        Players.Remove(player);

        if (HasEnoughPlayers)
        {
            return;
        }

        AddDomainEvent(new GameCompletedDomainEvent(GameMapper.GameToGameEventData(this)));
    }

    public Player? GetPlayer(string userId)
    {
        return Players.FirstOrDefault(p => p.UserId == userId);
    }
}