using Games.Domain.Entities;
using Games.Domain.Enums;
using Games.Domain.Errors;
using Games.Domain.Events;
using Games.Domain.Tests.Data;
using Games.Domain.Types;
using OneOf.Types;
using Shared.Domain.Enums;

namespace Games.Domain.Tests.Entities;

public sealed class GameTests
{
    // ------------------------------------------------------------------------
    // RemovePlayer
    // ------------------------------------------------------------------------

    [Theory]
    [ClassData(typeof(ValidGames))]
    public void GivenGame_WhenRemovePlayer_ThenReturnsSuccess(Game game)
    {
        // given
        var playerToRemove = game.Players.First();

        // when
        var result = game.RemovePlayer(playerToRemove.UserId);

        // then
        Assert.IsType<Success>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public void GivenGame_WhenRemovePlayer_ThenRemovesPlayerFromList(Game game)
    {
        // given
        var playerToRemove = game.Players.First();

        // when
        game.RemovePlayer(playerToRemove.UserId);

        // then
        Assert.DoesNotContain(playerToRemove, game.Players);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public void GivenGame_WhenRemovePlayer_ThenAddsPlayerLeftEvent(Game game)
    {
        // given
        var playerToRemove = game.Players.First();

        // when
        game.RemovePlayer(playerToRemove.UserId);

        // then
        var domainEvent = game.DomainEvents.OfType<PlayerLeftDomainEvent>().SingleOrDefault();

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal(playerToRemove.UserId, domainEvent?.Player.UserId)
        );
    }

    [Theory]
    [ClassData(typeof(GameWith2Players))]
    public void GivenGameWith2Players_WhenRemovePlayer_ThenAddsGameCompletedDomainEvent(Game game)
    {
        // given
        var playerToRemove = game.Players.First();

        // when
        game.RemovePlayer(playerToRemove.UserId);

        // then
        var domainEvent = game.DomainEvents.OfType<GameCompletedDomainEvent>().SingleOrDefault();

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal(game.Id, domainEvent?.Game.Id),
            () => Assert.Equal(game.LobbyId, domainEvent?.Game.LobbyId)
        );
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public void GivenGameNotContainingPlayer_WhenRemovePlayer_ThenReturnsPlayerNotInGameError(Game game)
    {
        // given
        var nonExistantUserid = "doesnt exist";

        // when
        var result = game.RemovePlayer(nonExistantUserid);

        // then
        Assert.IsType<PlayerNotInGameDomainError>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public void GivenGame_WhenRemoveCurrentPlayer_ThenEndsTurn(Game game)
    {
        // given
        var currentPlayer = game.CurrentPlayer;

        // when
        game.RemovePlayer(currentPlayer.UserId);

        // then
        Assert.Multiple(
            () => Assert.Equal(TurnPhase.PlaceTile, game.TurnPhase),
            () => Assert.NotEqual(currentPlayer, game.CurrentPlayer)
        );
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public void GivenGame_WhenRemoveCurrentPlayer_ThenAddsTurnCompletedDomainEvent(Game game)
    {
        // given
        var currentPlayer = game.CurrentPlayer;

        // when
        game.RemovePlayer(currentPlayer.UserId);

        // then
        var domainEvent = game.DomainEvents.OfType<TurnCompleteDomainEvent>().SingleOrDefault();

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal(game.Id, domainEvent?.GameId),
            () => Assert.Equal(game.CurrentPlayer.UserId, domainEvent?.NewPlayer.UserId)
        );
    }

    // ------------------------------------------------------------------------
    // DrawTile
    // ------------------------------------------------------------------------

    [Theory]
    [ClassData(typeof(PlaceTileGames))]
    public void GivenGameInPlaceTilePhase_WhenDrawTile_ThenReturnsTile(Game game)
    {
        // when
        var result = game.DrawTile();

        // then
        Assert.IsType<Tile>(result.Value, exactMatch: false);
    }

    [Theory]
    [ClassData(typeof(PlaceTileGames))]
    public void GivenGameInPlaceTilePhase_WhenDrawTile_ThenSetsPendingTileToDrawnTile(Game game)
    {
        // when
        var result = game.DrawTile();

        // then
        Assert.Multiple(
            () => Assert.NotNull(game.PendingTile),
            () => Assert.Equivalent(TileCatalogue.CreateTile(game.PendingTile!.Value, default), result.Value, strict: true)
        );
    }

    [Theory]
    [ClassData(typeof(PlaceTileGames))]
    public void GivenGameInPlaceTilePhase_WhenDrawTile_ThenIncludesPendingTileInDrawnTilesBitmask(Game game)
    {
        // given
        var drawnTilesBefore = game.DrawnTiles;

        // when
        game.DrawTile();

        // then
        var drawnBit = 1UL << (int)game.PendingTile!.Value;

        Assert.Multiple(
            () => Assert.Equal(0UL, drawnTilesBefore & drawnBit),
            () => Assert.Equal(drawnTilesBefore | drawnBit, game.DrawnTiles)
        );
    }

    [Theory]
    [ClassData(typeof(PlaceTileGames))]
    public void GivenGameInPlaceTilePhase_WhenDrawTileMultipleTimesWithoutPlacing_ThenReturnsSameTile(Game game)
    {
        // when
        var tile = game.DrawTile().Value;
        var tile1 = game.DrawTile().Value;
        var tile2 = game.DrawTile().Value;
        var tile3 = game.DrawTile().Value;

        // then
        Assert.Multiple(
            () => Assert.Equivalent(tile, tile1, strict: true),
            () => Assert.Equivalent(tile, tile2, strict: true),
            () => Assert.Equivalent(tile, tile3, strict: true)
        );
    }

    [Theory]
    [ClassData(typeof(ActionPhaseGames))]
    public void GivenGameinActionPhase_WhenDrawTile_ThenReturnsInvalidTurnPhaseError(Game game)
    {
        // when
        var result = game.DrawTile();

        // then
        Assert.IsType<InvalidTurnPhaseDomainError>(result.Value);
    }

    // ------------------------------------------------------------------------
    // PlaceTile
    // The logic if a tile can or can not be placed in a given grid is tested 
    // as part of the Tile tests. Here we only test the consequences.
    // ------------------------------------------------------------------------

    [Theory]
    [ClassData(typeof(ActionPhaseGames))]
    public void GivenGameinActionPhase_WhenPlaceTile_ThenReturnsInvalidTurnPhaseError(Game game)
    {
        // when
        var result = game.PlaceTile(0, 0, 0);

        // then
        Assert.IsType<InvalidTurnPhaseDomainError>(result.Value);
    }

    [Theory]
    [ClassData(typeof(PlaceTileGames))]
    public void GivenGameWithOutDrawing_WhenPlaceTile_ThenReturnsTileMustBeDrawnFirstError(Game game)
    {
        // when
        var result = game.PlaceTile(0, 0, 0);

        // then
        Assert.IsType<TileMustBeDrawnFirstDomainError>(result.Value);
    }

    [Theory]
    [ClassData(typeof(PlaceTileGames))]
    public void GivenInvalidTilePosition_WhenPlaceTile_ThenReturnsTileCannotBePlacedError(Game game)
    {
        // give
        game.DrawTile();

        // when
        var result = game.PlaceTile(3, -4, 0);

        // then
        Assert.IsType<TileCannotBePlacedDomainError>(result.Value);
    }

    [Fact]
    public void GivenValidTilePosition_WhenPlaceTile_ThenReturnsSuccess()
    {
        // given
        var game = GameWithPendingTile();

        // when
        var result = game.PlaceTile(1, -2, 0);

        // then
        Assert.IsType<Success>(result.Value);
    }

    [Fact]
    public void GivenValidTilePosition_WhenPlaceTile_ThenAddsTileAndAdvancesPhase()
    {
        // given
        var game = GameWithPendingTile();

        // when
        game.PlaceTile(1, -2, 0);

        // then
        Assert.Multiple(
            () => Assert.Null(game.PendingTile),
            () => Assert.Equal(TurnPhase.Actions, game.TurnPhase),
            () => Assert.Contains(game.Tiles, t => t.Coordinate == new HexCoordinate(1, -2))
        );
    }

    /// <summary>
    /// A game in the place tile phase where A1 (temple, south 2) has been drawn.
    /// (1, -2) is a valid place for it: its south edge faces the tile on (1, -1).
    /// </summary>
    private static Game GameWithPendingTile()
    {
        return new Game
        {
            LobbyId = 1,
            TurnPhase = TurnPhase.PlaceTile,
            PendingTile = TileCardIdentifier.A1,
            DrawnTiles = 1UL << (int)TileCardIdentifier.A1,
            Players =
            [
                new Player
                {
                    UserId = "ebb73b46-d8fd-460e-ae24-848d8c20141f",
                    Name = "Player1",
                    Colour = Colour.Red,
                    Points = 0,
                    TurnPosition = 0
                },
                new Player
                {
                    UserId = "1a6a2a19-d2a3-46eb-b732-da1550ccc8d1",
                    Name = "Player2",
                    Colour = Colour.Black,
                    Points = 0,
                    TurnPosition = 1
                }
            ],
            Tiles =
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(NorthEast: 1),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(SouthWest: 1),
                    Coordinate = new HexCoordinate(1, -1)
                }
            ]
        };
    }
}