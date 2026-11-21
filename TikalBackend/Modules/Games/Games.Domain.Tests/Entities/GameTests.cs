using Games.Domain.Entities;
using Games.Domain.Enums;
using Games.Domain.Errors;
using Games.Domain.Events;
using Games.Domain.Tests.Data;
using OneOf.Types;

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
}