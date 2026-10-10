using Lobbies.Domain.Entities;
using Lobbies.Domain.Events;
using Lobbies.Domain.Tests.Data;
using Shared.Domain.Enums;

namespace Lobbies.Domain.Tests.Entities;

public sealed class LobbyTests
{
    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public void GivenLobby_WhenRemovePlayer_ThenRemovesPlayerFromList(Lobby lobby)
    {
        // given
        var playerToRemove = lobby.Players.First();

        // when
        lobby.RemovePlayer(playerToRemove);

        // then
        Assert.DoesNotContain(playerToRemove, lobby.Players);
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public void GivenLobby_WhenRemovePlayer_ThenAddsPlayerLeftEvent(Lobby lobby)
    {
        // given
        var playerToRemove = lobby.Players.First();

        // when
        lobby.RemovePlayer(playerToRemove);

        // then
        var domainEvent = lobby.DomainEvents.OfType<PlayerLeftEvent>().SingleOrDefault();

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal(playerToRemove, domainEvent?.Player)
        );
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public void GivenLobby_WhenAddPlayer_ThenAddsPlayerToList(Lobby lobby)
    {
        // given
        var playerToAdd = new Player
        {
            UserId = "user-id",
            IsOwner = false,
            IsReady = false,
            SelectedColour = Colour.Red
        };

        // when
        lobby.AddPlayer(playerToAdd);

        // then
        Assert.Contains(playerToAdd, lobby.Players);
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public void GivenLobby_WhenAddPlayer_ThenAddsPlayerJoinedEvent(Lobby lobby)
    {
        // given
        var playerToAdd = new Player
        {
            UserId = "user-id",
            IsOwner = false,
            IsReady = false,
            SelectedColour = Colour.Red
        };

        // when
        lobby.AddPlayer(playerToAdd);

        // then
        var domainEvent = lobby.DomainEvents.OfType<PlayerJoinedEvent>().SingleOrDefault();

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal(playerToAdd, domainEvent?.Player)
        );
    }

    [Theory]
    [ClassData(typeof(LobbiesWithOneOwnerAndMultiplePlayers))]
    public void GivenLobbyWithMultiplePlayersAndOneOwner_WhenOwnerIsRemoved_ThenPromotesAnotherPlayerToOwner(
        Lobby lobby
    )
    {
        // given
        var playerToRemove = lobby.Players.First(p => p.IsOwner);

        // when
        lobby.RemovePlayer(playerToRemove);

        // then
        Assert.Multiple(
            () => Assert.DoesNotContain(playerToRemove, lobby.Players),
            () => Assert.Contains(lobby.Players, p => p.IsOwner)
        );
    }

    [Theory]
    [ClassData(typeof(LobbiesWithOneOwnerAndMultiplePlayers))]
    public void GivenLobbyWithMultiplePlayersAndOneOwner_WhenOwnerIsRemoved_ThenAddsPlayerUpdatedEvent(
        Lobby lobby
    )
    {
        // given
        var playerToRemove = lobby.Players.First(p => p.IsOwner);

        // when
        lobby.RemovePlayer(playerToRemove);

        // then
        var domainEvent = lobby.DomainEvents.OfType<PlayerUpdatedEvent>().SingleOrDefault();
        var promotedPlayer = lobby.Players.First(p => p.IsOwner);

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal(promotedPlayer, domainEvent?.Player)
        );
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public void GivenLobby_WhenGetUnusedColour_ThenReturnsColourUsedByNoPlayer(Lobby lobby)
    {
        // given
        var usedColours = lobby.Players.Select(p => p.SelectedColour).ToHashSet();

        // when
        var unusedColour = lobby.GetUnusedColour();

        // then
        Assert.DoesNotContain(unusedColour, usedColours);
    }
}