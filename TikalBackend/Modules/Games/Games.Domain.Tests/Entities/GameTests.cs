using Games.Domain.Entities;
using Games.Domain.Errors;
using Games.Domain.Events;
using Games.Domain.Tests.Data;
using OneOf.Types;

namespace Games.Domain.Tests.Entities;

public sealed class GameTests
{
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
}