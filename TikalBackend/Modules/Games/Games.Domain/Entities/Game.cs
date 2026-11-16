using System.Numerics;
using Games.Domain.Enums;
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

    public TurnPhase TurnPhase { get; set; }

    /// <summary>
    /// Indicates which player's turn it is currently.
    /// Matches <see cref="Player.TurnPosition"/>.
    /// </summary>
    public int CurrentTurnPosition { get; set; }

    public uint Version { get; set; }

    public bool HasEnoughPlayers => Players.Count > 1;

    public OneOf<Success, PlayerNotInGameDomainError> RemovePlayer(string userId)
    {
        var player = Players.FirstOrDefault(p => p.UserId == userId);

        if (player is null)
        {
            return new PlayerNotInGameDomainError();
        }

        Players.Remove(player);
        Touch();
        AddDomainEvent(new PlayerLeftDomainEvent(PlayerMapper.PlayerToPlayerEventData(player), LobbyId));

        if (HasEnoughPlayers)
        {
            return new Success();
        }

        AddDomainEvent(new GameCompletedDomainEvent(GameMapper.GameToGameEventData(this)));

        return new Success();
    }

    /// <summary>
    /// Draws the next tile of the card stack for this game.
    /// Returns null if all tiles have been drawn already.
    /// </summary>
    public Tile? DrawTile()
    {
        if (PendingTile is not null)
        {
            return TileCatalogue.Tiles[PendingTile.Value];
        }

        foreach (var bucket in TileCatalogue.BucketBitMasks)
        {
            var remaining = bucket & ~DrawnTiles;

            if (remaining == 0)
            {
                continue;
            }

            // Remove a random number of trailing 1s of this bucket.
            // The last trailing 1 of this bucket will be the chosen tile.
            for (var skip = Random.Shared.Next(BitOperations.PopCount(remaining)); skip > 0; --skip)
            {
                remaining &= remaining - 1;
            }

            var tileIdentifier = (TileCardIdentifier)BitOperations.TrailingZeroCount(remaining);

            PendingTile = tileIdentifier;
            DrawnTiles |= 1UL << (int)tileIdentifier;

            return TileCatalogue.Tiles[tileIdentifier];
        }

        return null;
    }

    public Player? GetPlayer(string userId)
    {
        return Players.FirstOrDefault(p => p.UserId == userId);
    }
}