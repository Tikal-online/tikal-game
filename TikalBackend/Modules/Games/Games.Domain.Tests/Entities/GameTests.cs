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

    // ------------------------------------------------------------------------
    // DrawTile
    // ------------------------------------------------------------------------

    [Theory]
    [ClassData(typeof(ValidGames))]
    public void GivenGameWithTilesLeft_WhenDrawTile_ThenReturnsNotNullTile(Game game)
    {
        // when
        var tile = game.DrawTile();

        // then
        Assert.NotNull(tile);
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public void GivenGameWithTilesLeft_WhenDrawTile_ThenSetsPendingTileToDrawnTile(Game game)
    {
        // when
        var tile = game.DrawTile();

        // then
        Assert.Multiple(
            () => Assert.NotNull(game.PendingTile),
            () => Assert.Equal(TileCatalogue.Tiles[game.PendingTile!.Value], tile)
        );
    }

    [Theory]
    [ClassData(typeof(ValidGames))]
    public void GivenGameWithTilesLeft_WhenDrawTile_ThenIncludesPendingTileInDrawnTilesBitmask(Game game)
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
    [ClassData(typeof(ValidGames))]
    public void GivenGameWithTilesLeft_WhenDrawTileMultipleTimesWithoutPlacing_ThenReturnsSameTile(Game game)
    {
        // when
        var tile = game.DrawTile();
        var tile1 = game.DrawTile();
        var tile2 = game.DrawTile();
        var tile3 = game.DrawTile();

        // then
        Assert.Multiple(
            () => Assert.Equal(tile, tile1),
            () => Assert.Equal(tile, tile2),
            () => Assert.Equal(tile, tile3)
        );
    }
}