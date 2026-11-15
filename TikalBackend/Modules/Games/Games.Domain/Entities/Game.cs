using Games.Domain.Errors;
using Games.Domain.Events;
using Games.Domain.Mappers;
using OneOf;
using OneOf.Types;
using Shared.Domain.Entities;

namespace Games.Domain.Entities;

public sealed class Game : Entity
{
    public long Id { get; set; }

    public required long LobbyId { get; set; }

    public ICollection<Player> Players { get; set; } = [];

    public List<Tile> Tiles { get; set; } = [];

    /// <summary>
    /// A bit mask that holds which tiles have already been drawn and played.
    /// </summary>
    public ulong DrawnTiles { get; set; }

    /// <summary>
    /// The identifier of the tile that has been drawn, but not yet played.
    /// Ensures that the same tile gets drawn if a player draws
    /// multiple times without placing.
    /// </summary>
    public TileCardIdentifier? PendingTile { get; set; }

    public bool HasEnoughPlayers => Players.Count > 1;

    public OneOf<Success, PlayerNotInGameDomainError> RemovePlayer(string userId)
    {
        var player = Players.FirstOrDefault(p => p.UserId == userId);

        if (player is null)
        {
            return new PlayerNotInGameDomainError();
        }

        Players.Remove(player);
        AddDomainEvent(new PlayerLeftDomainEvent(PlayerMapper.PlayerToPlayerEventData(player), LobbyId));

        if (HasEnoughPlayers)
        {
            return new Success();
        }

        AddDomainEvent(new GameCompletedDomainEvent(GameMapper.GameToGameEventData(this)));

        return new Success();
    }

    public Player? GetPlayer(string userId)
    {
        return Players.FirstOrDefault(p => p.UserId == userId);
    }
}