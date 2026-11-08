using Lobbies.Domain.Entities;
using Lobbies.Domain.Errors;
using Lobbies.Domain.Events;
using Lobbies.Domain.Tests.Data;
using OneOf.Types;

namespace Lobbies.Domain.Tests.Entities;

public sealed class LobbyTests
{
    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public void GivenLobby_WhenRemovePlayer_ThenReturnsSuccess(Lobby lobby)
    {
        // given
        var playerToRemove = lobby.Players.First();

        // when
        var result = lobby.RemovePlayer(playerToRemove.UserId);

        // then
        Assert.IsType<Success>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public void GivenLobby_WhenRemovePlayer_ThenRemovesPlayerFromList(Lobby lobby)
    {
        // given
        var playerToRemove = lobby.Players.First();

        // when
        lobby.RemovePlayer(playerToRemove.UserId);

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
        lobby.RemovePlayer(playerToRemove.UserId);

        // then
        var domainEvent = lobby.DomainEvents.OfType<PlayerLeftDomainEvent>().SingleOrDefault();

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal(playerToRemove.UserId, domainEvent?.Player.UserId)
        );
    }

    [Theory]
    [ClassData(typeof(InGameLobbies))]
    public void GivenLobbyInGame_WhenRemovePlayer_ThenReturnsInGameError(Lobby lobby)
    {
        // given
        var playerToRemove = lobby.Players.First();

        // when
        var result = lobby.RemovePlayer(playerToRemove.UserId);

        // then
        Assert.IsType<LobbyInGameDomainError>(result.Value);
    }

    [Theory]
    [ClassData(typeof(ValidLobbies))]
    public void GivenLobbyNotContainingPlayer_WhenRemovePlayer_ThenReturnsPlayerNotInLobbyError(Lobby lobby)
    {
        // given
        var nonExistantUserId = "doesnt exist";

        // when
        var result = lobby.RemovePlayer(nonExistantUserId);

        // PlayerNotInLobbyDomainError
        Assert.IsType<PlayerNotInLobbyDomainError>(result.Value);
    }

    [Theory]
    [ClassData(typeof(NotFullLobbies))]
    public void GivenLobby_WhenAddPlayer_ThenReturnsSuccess(Lobby lobby)
    {
        // when
        var result = lobby.AddPlayer("username", "user-id");

        // then
        Assert.IsType<Success>(result.Value);
    }

    [Theory]
    [ClassData(typeof(NotFullLobbies))]
    public void GivenLobby_WhenAddPlayer_ThenAddsPlayerToList(Lobby lobby)
    {
        // when
        lobby.AddPlayer("username", "user-id");

        // then
        Assert.Contains(lobby.Players, p => p.UserId == "user-id" && p.Name == "username");
    }

    [Theory]
    [ClassData(typeof(NotFullLobbies))]
    public void GivenLobby_WhenAddPlayer_ThenAddsPlayerJoinedEvent(Lobby lobby)
    {
        // when
        lobby.AddPlayer("username", "user-id");

        // then
        var domainEvent = lobby.DomainEvents.OfType<PlayerJoinedDomainEvent>().SingleOrDefault();

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal("user-id", domainEvent?.Player.UserId)
        );
    }

    [Theory]
    [ClassData(typeof(InGameLobbies))]
    public void GivenInGameLobby_WhenAddPlayer_ThenReturnsLobbyInGameError(Lobby lobby)
    {
        // when
        var result = lobby.AddPlayer("username", "user-id");

        // then
        Assert.IsType<LobbyInGameDomainError>(result.Value);
    }

    [Theory]
    [ClassData(typeof(FullLobbies))]
    public void GivenFullLobby_WhenAddPlayer_ThenReturnsLobbyFullError(Lobby lobby)
    {
        // when
        var result = lobby.AddPlayer("username", "user-id");

        // then
        Assert.IsType<LobbyFullDomainError>(result.Value);
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
        lobby.RemovePlayer(playerToRemove.UserId);

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
        lobby.RemovePlayer(playerToRemove.UserId);

        // then
        var domainEvent = lobby.DomainEvents.OfType<PlayerUpdatedDomainEvent>().SingleOrDefault();
        var promotedPlayer = lobby.Players.First(p => p.IsOwner);

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal(promotedPlayer.UserId, domainEvent?.Player.UserId)
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

    [Theory]
    [ClassData(typeof(StartableLobbies))]
    public void GivenLobby_WhenStart_ThenReturnsSuccess(Lobby lobby)
    {
        // when
        var result = lobby.Start();

        // then;
        Assert.IsType<Success>(result.Value);
    }

    [Theory]
    [ClassData(typeof(StartableLobbies))]
    public void GivenLobby_WhenStart_ThenSetsInGameToTrue(Lobby lobby)
    {
        // when
        lobby.Start();

        // then;
        Assert.True(lobby.InGame);
    }

    [Theory]
    [ClassData(typeof(StartableLobbies))]
    public void GivenLobby_WhenStart_ThenAddsLobbyStartedEvent(Lobby lobby)
    {
        // when
        lobby.Start();

        // then
        var domainEvent = lobby.DomainEvents.OfType<LobbyStartedDomainEvent>().SingleOrDefault();

        Assert.Multiple(
            () => Assert.NotNull(domainEvent),
            () => Assert.Equal(lobby.Id, domainEvent?.Lobby.Id)
        );
    }

    [Theory]
    [ClassData(typeof(NotStartableLobbies))]
    public void GivenNotStartableLobby_WhenStart_ThenReturnsLobbyCannotBeStartedError(Lobby lobby)
    {
        // when
        var result = lobby.Start();

        // then;
        Assert.IsType<LobbyCannotBeStartedDomainError>(result.Value);
    }

    [Theory]
    [ClassData(typeof(InGameLobbies))]
    public void GivenLobby_WhenStop_ThenSetsInGameToFalse(Lobby lobby)
    {
        // when
        lobby.Stop();

        // thenFalse
        Assert.False(lobby.InGame);
    }

    [Theory]
    [ClassData(typeof(InGameLobbies))]
    public void GivenLobby_WhenStop_ThenSetsAllPlayersToNotReady(Lobby lobby)
    {
        // when
        lobby.Stop();

        // thenFalse
        Assert.DoesNotContain(lobby.Players, p => p.IsReady);
    }
}