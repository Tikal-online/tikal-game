using Lobbies.Domain.Entities;
using Lobbies.Domain.Events;
using Lobbies.Domain.Tests.Data;

namespace Lobbies.Domain.Tests.Entities;

internal sealed class LobbyTests
{
    // test data
    public static IEnumerable<Lobby> LobbyWithMultiplePlayersAndOneOwnerTestCases =>
        LobbyTestCases.ValidLobbyTestCases.Where(l => l.Players.Count > 1 && l.Players.Count(p => p.IsOwner) == 1);

    [TestCaseSource(typeof(LobbyTestCases), nameof(LobbyTestCases.ValidLobbyTestCases))]
    public void GivenLobby_WhenRemovePlayer_ThenRemovesPlayerFromList(Lobby lobby)
    {
        // given
        var playerToRemove = lobby.Players.First();

        // when
        lobby.RemovePlayer(playerToRemove);

        // then
        Assert.That(lobby.Players, Does.Not.Contain(playerToRemove));
    }

    [TestCaseSource(typeof(LobbyTestCases), nameof(LobbyTestCases.ValidLobbyTestCases))]
    public void GivenLobby_WhenRemovePlayer_ThenAddsPlayerLeftEvent(Lobby lobby)
    {
        // given
        var playerToRemove = lobby.Players.First();

        // when
        lobby.RemovePlayer(playerToRemove);

        // then
        var domainEvent = lobby.DomainEvents.OfType<PlayerLeftEvent>().SingleOrDefault();

        Assert.That(domainEvent, Is.Not.Null);
        Assert.That(domainEvent.Player, Is.EqualTo(playerToRemove));
    }

    [TestCaseSource(nameof(LobbyWithMultiplePlayersAndOneOwnerTestCases))]
    public void GivenLobbyWithMultiplePlayersAndOneOwner_WhenOwnerIsRemoved_ThenPromotesAnotherPlayerToOwner(
        Lobby lobby
    )
    {
        // given
        var playerToRemove = lobby.Players.First(p => p.IsOwner);

        // when
        lobby.RemovePlayer(playerToRemove);

        // then
        Assert.That(lobby.Players, Does.Not.Contain(playerToRemove));
        Assert.That(lobby.Players.Any(p => p.IsOwner), Is.True);
    }

    [TestCaseSource(typeof(LobbyTestCases), nameof(LobbyTestCases.ValidLobbyTestCases))]
    public void GivenLobby_WhenGetUnusedColour_ThenReturnsColourUsedByNoPlayer(Lobby lobby)
    {
        // given
        var usedColours = lobby.Players.Select(p => p.SelectedColour).ToHashSet();

        // when
        var unusedColour = lobby.GetUnusedColour();

        // then
        Assert.That(usedColours.Contains(unusedColour), Is.False);
    }
}