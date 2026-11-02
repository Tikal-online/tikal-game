using Games.Domain.Entities;
using Games.Domain.Events;
using Games.Domain.Tests.Data;

namespace Games.Domain.Tests.Entities;

public sealed class GameTests
{
    [Theory]
    [ClassData(typeof(ValidGames))]
    public void GivenGame_WhenRemovePlayer_ThenRemovesPlayerFromList(Game game)
    {
        // given
        var playerToRemove = game.Players.First();

        // when
        game.RemovePlayer(playerToRemove);

        // then
        Assert.DoesNotContain(playerToRemove, game.Players);
    }

    [Theory]
    [ClassData(typeof(GameWith2Players))]
    public void GivenGameWith2Players_WhenRemovePlayer_ThenAddsGameCompletedDomainEvent(Game game)
    {
        // given
        var playerToRemove = game.Players.First();

        // when
        game.RemovePlayer(playerToRemove);

        // then
        var domainEvent = game.DomainEvents.OfType<GameCompletedDomainEvent>().SingleOrDefault();

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal(game.Id, domainEvent?.Game.Id),
            () => Assert.Equal(game.LobbyId, domainEvent?.Game.LobbyId)
        );
    }
}